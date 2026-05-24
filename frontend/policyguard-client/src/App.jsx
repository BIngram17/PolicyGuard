import { useEffect, useState } from "react";
import { apiClient } from "./api/apiClient";
import Header from "./components/Header";
import LoginPage from "./components/LoginPage";
import AuditTrailPage from "./pages/AuditTrailPage";
import ChecklistDetailPage from "./pages/ChecklistDetailPage";
import ChecklistsPage from "./pages/ChecklistsPage";
import DashboardPage from "./pages/DashboardPage";
import NewReviewPage from "./pages/NewReviewPage";
import ReportPage from "./pages/ReportPage";
import ResultsPage from "./pages/ResultsPage";
import SavedReviewsPage from "./pages/SavedReviewsPage";
import { clearAuth, getSavedAuth, saveAuth } from "./utils/auth";
import { buildTextReport } from "./utils/reportBuilder";
import "./App.css";

const emptyChecklistItem = {
  requirement: "",
  description: "",
  keywords: "",
  weight: 10,
};

function App() {
  const [activePage, setActivePage] = useState("dashboard");
  const [dashboard, setDashboard] = useState(null);
  const [checklists, setChecklists] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [auditLogs, setAuditLogs] = useState([]);
  const [selectedReview, setSelectedReview] = useState(null);
  const [selectedChecklist, setSelectedChecklist] = useState(null);
  const [loading, setLoading] = useState(false);
  const [reviewFilter, setReviewFilter] = useState("all");
  const [auditFilter, setAuditFilter] = useState("all");
  const [lastFullRefreshAt, setLastFullRefreshAt] = useState(null);
  const [lastAuditRefreshAt, setLastAuditRefreshAt] = useState(null);
  const [isEditingChecklist, setIsEditingChecklist] = useState(false);

  const [currentUser, setCurrentUser] = useState(() => getSavedAuth());

  const [loginForm, setLoginForm] = useState({
    email: "",
    password: "",
  });

  const [reviewForm, setReviewForm] = useState({
    title: "",
    complianceChecklistId: "",
    documentText: "",
  });

  const [checklistForm, setChecklistForm] = useState({
    name: "",
    description: "",
    category: "",
    items: [{ ...emptyChecklistItem }],
  });

  const [editChecklistForm, setEditChecklistForm] = useState({
    name: "",
    description: "",
    category: "",
    items: [{ ...emptyChecklistItem }],
  });

  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const isAdmin = currentUser?.role === "Admin";
  const isReviewer = currentUser?.role === "Reviewer";
  const isAuditor = currentUser?.role === "Auditor";

  const canCreateReviews = isAdmin || isReviewer;
  const canManageChecklists = isAdmin;
  const canDeleteReviews = isAdmin;
  const canViewAuditTrail = isAdmin || isAuditor;

  useEffect(() => {
    if (currentUser) {
      loadInitialData(currentUser);
    }
  }, [currentUser]);

  function handleAuthError(err) {
    if (err.response?.status === 401) {
      handleLogout();
      setError("Your session expired or is invalid. Please log in again.");
      return true;
    }

    if (err.response?.status === 403) {
      setError("You are logged in, but your role does not allow this action.");
      return true;
    }

    return false;
  }

  async function handleLogin(event) {
    event.preventDefault();
    setError("");
    setSuccessMessage("");
    setLoading(true);

    try {
      const response = await apiClient.login({
        email: loginForm.email,
        password: loginForm.password,
      });

      const authData = response.data;
      saveAuth(authData);
      setCurrentUser(authData);
      setActivePage("dashboard");
      setSuccessMessage(`Logged in as ${authData.fullName}.`);
    } catch (err) {
      console.error(err);
      setError(err.response?.data?.message || "Login failed.");
    } finally {
      setLoading(false);
    }
  }

  function handleLogout() {
    clearAuth();
    setCurrentUser(null);
    setDashboard(null);
    setChecklists([]);
    setReviews([]);
    setAuditLogs([]);
    setSelectedReview(null);
    setSelectedChecklist(null);
    setLastFullRefreshAt(null);
    setLastAuditRefreshAt(null);
    setIsEditingChecklist(false);
    setActivePage("dashboard");
    setLoginForm({
      email: "",
      password: "",
    });
  }

  async function loadInitialData(user = currentUser) {
    if (!user) {
      return;
    }

    setError("");

    try {
      const dashboardRequest = apiClient.getDashboard(user);
      const checklistsRequest = apiClient.getChecklists(user);
      const reviewsRequest = apiClient.getReviews(user);

      const requests = [dashboardRequest, checklistsRequest, reviewsRequest];

      const shouldLoadAuditLogs = user.role === "Admin" || user.role === "Auditor";

      if (shouldLoadAuditLogs) {
        requests.push(apiClient.getAuditLogs(user));
      }

      const responses = await Promise.all(requests);

      setDashboard(responses[0].data);
      setChecklists(responses[1].data);
      setReviews(responses[2].data);

      const refreshedAt = new Date();
      setLastFullRefreshAt(refreshedAt);

      if (shouldLoadAuditLogs) {
        setAuditLogs(responses[3].data);
        setLastAuditRefreshAt(refreshedAt);
      } else {
        setAuditLogs([]);
        setLastAuditRefreshAt(null);
      }
    } catch (err) {
      console.error(err);

      if (!handleAuthError(err)) {
        setError(
          "Could not load data from the API. Make sure the backend is running on http://localhost:5069."
        );
      }
    }
  }

  async function loadAuditLogs() {
    if (!canViewAuditTrail) {
      setError("Your role does not allow access to the audit trail.");
      return;
    }

    try {
      const response = await apiClient.getAuditLogs(currentUser);
      setAuditLogs(response.data);
      setLastAuditRefreshAt(new Date());
    } catch (err) {
      console.error(err);

      if (!handleAuthError(err)) {
        setError("Could not load audit logs.");
      }
    }
  }

  async function handleAnalyzePolicy(event) {
    event.preventDefault();
    setError("");
    setSuccessMessage("");
    setLoading(true);
    setSelectedReview(null);

    if (!canCreateReviews) {
      setError("Your role does not allow creating policy reviews.");
      setLoading(false);
      return;
    }

    if (!reviewForm.title.trim()) {
      setError("Please enter a review title.");
      setLoading(false);
      return;
    }

    if (!reviewForm.complianceChecklistId) {
      setError("Please select a compliance checklist.");
      setLoading(false);
      return;
    }

    if (!reviewForm.documentText.trim()) {
      setError("Please paste policy or procedure text to analyze.");
      setLoading(false);
      return;
    }

    try {
      const response = await apiClient.analyzePolicy(
        {
          title: reviewForm.title,
          complianceChecklistId: Number(reviewForm.complianceChecklistId),
          documentText: reviewForm.documentText,
        },
        currentUser
      );

      setSelectedReview(response.data);
      setReviewForm({
        title: "",
        complianceChecklistId: "",
        documentText: "",
      });

      await loadInitialData();
      setActivePage("results");
    } catch (err) {
      console.error(err);

      if (!handleAuthError(err)) {
        setError(
          err.response?.data?.message ||
            "The policy review could not be completed."
        );
      }
    } finally {
      setLoading(false);
    }
  }

  async function handleViewReview(reviewId) {
    setError("");
    setSuccessMessage("");
    setLoading(true);

    try {
      const response = await apiClient.getReviewById(reviewId, currentUser);
      setSelectedReview(response.data);
      setActivePage("results");
    } catch (err) {
      console.error(err);

      if (!handleAuthError(err)) {
        setError("Could not load the selected review.");
      }
    } finally {
      setLoading(false);
    }
  }

  async function handleDeleteReview(reviewId) {
    if (!canDeleteReviews) {
      setError("Only Admin users can delete policy reviews.");
      return;
    }

    const confirmed = window.confirm(
      "Delete this policy review? This will remove the saved review and its results."
    );

    if (!confirmed) {
      return;
    }

    setError("");
    setSuccessMessage("");
    setLoading(true);

    try {
      await apiClient.deleteReview(reviewId, currentUser);

      if (selectedReview?.id === reviewId) {
        setSelectedReview(null);
      }

      await loadInitialData();
      setSuccessMessage("Policy review deleted successfully.");
      setActivePage("saved-reviews");
    } catch (err) {
      console.error(err);

      if (!handleAuthError(err)) {
        setError("Could not delete the selected review.");
      }
    } finally {
      setLoading(false);
    }
  }

  async function handleViewChecklist(checklistId) {
    setError("");
    setSuccessMessage("");
    setIsEditingChecklist(false);
    setLoading(true);

    try {
      const response = await apiClient.getChecklistById(
        checklistId,
        currentUser
      );

      setSelectedChecklist(response.data);
      setActivePage("checklist-detail");
    } catch (err) {
      console.error(err);

      if (!handleAuthError(err)) {
        setError("Could not load the selected checklist.");
      }
    } finally {
      setLoading(false);
    }
  }

  async function handleCreateChecklist(event) {
    event.preventDefault();
    setError("");
    setSuccessMessage("");
    setLoading(true);

    if (!canManageChecklists) {
      setError("Only Admin users can create checklist templates.");
      setLoading(false);
      return;
    }

    const cleanedItems = checklistForm.items.map((item) => ({
      requirement: item.requirement.trim(),
      description: item.description.trim(),
      keywords: item.keywords.trim(),
      weight: Number(item.weight) || 10,
    }));

    const payload = {
      name: checklistForm.name.trim(),
      description: checklistForm.description.trim(),
      category: checklistForm.category.trim(),
      items: cleanedItems,
    };

    if (!payload.name || !payload.description || !payload.category) {
      setError("Checklist name, description, and category are required.");
      setLoading(false);
      return;
    }

    if (payload.items.length === 0) {
      setError("At least one checklist item is required.");
      setLoading(false);
      return;
    }

    const hasInvalidItem = payload.items.some(
      (item) => !item.requirement || !item.description || !item.keywords
    );

    if (hasInvalidItem) {
      setError(
        "Each checklist item must include a requirement, description, and keywords."
      );
      setLoading(false);
      return;
    }

    try {
      await apiClient.createChecklist(payload, currentUser);

      setChecklistForm({
        name: "",
        description: "",
        category: "",
        items: [{ ...emptyChecklistItem }],
      });

      setSelectedChecklist(null);
      await loadInitialData();
      setSuccessMessage("Checklist created successfully.");
    } catch (err) {
      console.error(err);

      if (!handleAuthError(err)) {
        setError(err.response?.data?.message || "Could not create the checklist.");
      }
    } finally {
      setLoading(false);
    }
  }

  function handleStartEditChecklist() {
    if (!selectedChecklist) {
      return;
    }

    setEditChecklistForm({
      name: selectedChecklist.name || "",
      description: selectedChecklist.description || "",
      category: selectedChecklist.category || "",
      items:
        selectedChecklist.items?.map((item) => ({
          requirement: item.requirement || "",
          description: item.description || "",
          keywords: item.keywords || "",
          weight: item.weight || 10,
        })) || [{ ...emptyChecklistItem }],
    });

    setError("");
    setSuccessMessage("");
    setIsEditingChecklist(true);
  }

  function handleCancelEditChecklist() {
    setIsEditingChecklist(false);
    setEditChecklistForm({
      name: "",
      description: "",
      category: "",
      items: [{ ...emptyChecklistItem }],
    });
  }

  function handleEditChecklistFieldChange(field, value) {
    setEditChecklistForm({
      ...editChecklistForm,
      [field]: value,
    });
  }

  function handleEditChecklistItemChange(index, field, value) {
    const updatedItems = editChecklistForm.items.map((item, itemIndex) => {
      if (itemIndex !== index) {
        return item;
      }

      return {
        ...item,
        [field]: field === "weight" ? Number(value) : value,
      };
    });

    setEditChecklistForm({
      ...editChecklistForm,
      items: updatedItems,
    });
  }

  function handleAddEditChecklistItem() {
    setEditChecklistForm({
      ...editChecklistForm,
      items: [...editChecklistForm.items, { ...emptyChecklistItem }],
    });
  }

  function handleRemoveEditChecklistItem(index) {
    if (editChecklistForm.items.length === 1) {
      setError("At least one checklist item is required.");
      return;
    }

    setEditChecklistForm({
      ...editChecklistForm,
      items: editChecklistForm.items.filter(
        (_, itemIndex) => itemIndex !== index
      ),
    });
  }

  async function handleUpdateChecklist(event) {
    event.preventDefault();

    if (!selectedChecklist?.id) {
      setError("No checklist template is selected.");
      return;
    }

    if (!canManageChecklists) {
      setError("Only Admin users can update checklist templates.");
      return;
    }

    setError("");
    setSuccessMessage("");
    setLoading(true);

    const cleanedItems = editChecklistForm.items.map((item) => ({
      requirement: item.requirement.trim(),
      description: item.description.trim(),
      keywords: item.keywords.trim(),
      weight: Number(item.weight) || 10,
    }));

    const payload = {
      name: editChecklistForm.name.trim(),
      description: editChecklistForm.description.trim(),
      category: editChecklistForm.category.trim(),
      items: cleanedItems,
    };

    if (!payload.name || !payload.description || !payload.category) {
      setError("Checklist name, description, and category are required.");
      setLoading(false);
      return;
    }

    if (payload.items.length === 0) {
      setError("At least one checklist item is required.");
      setLoading(false);
      return;
    }

    const hasInvalidItem = payload.items.some(
      (item) => !item.requirement || !item.description || !item.keywords
    );

    if (hasInvalidItem) {
      setError(
        "Each checklist item must include a requirement, description, and keywords."
      );
      setLoading(false);
      return;
    }

    try {
      await apiClient.updateChecklist(selectedChecklist.id, payload, currentUser);

      const updatedChecklistResponse = await apiClient.getChecklistById(
        selectedChecklist.id,
        currentUser
      );

      setSelectedChecklist(updatedChecklistResponse.data);
      setIsEditingChecklist(false);
      await loadInitialData();
      setSuccessMessage("Checklist template updated successfully.");
    } catch (err) {
      console.error(err);

      if (!handleAuthError(err)) {
        setError(
          err.response?.data?.message ||
            "Could not update the selected checklist template."
        );
      }
    } finally {
      setLoading(false);
    }
  }

  async function handleDeleteChecklist(checklistId) {
    if (!canManageChecklists) {
      setError("Only Admin users can delete checklist templates.");
      return;
    }

    const confirmed = window.confirm(
      "Delete this checklist? Checklists already used by saved reviews cannot be deleted."
    );

    if (!confirmed) {
      return;
    }

    setError("");
    setSuccessMessage("");
    setLoading(true);

    try {
      await apiClient.deleteChecklist(checklistId, currentUser);

      if (selectedChecklist?.id === checklistId) {
        setSelectedChecklist(null);
      }

      setIsEditingChecklist(false);
      await loadInitialData();
      setSuccessMessage("Checklist deleted successfully.");
      setActivePage("checklists");
    } catch (err) {
      console.error(err);

      if (!handleAuthError(err)) {
        setError(
          err.response?.data?.message || "Could not delete the selected checklist."
        );
      }
    } finally {
      setLoading(false);
    }
  }

  async function logReportExport() {
    if (!selectedReview?.id) {
      return;
    }

    try {
      await apiClient.logReportExport(selectedReview.id, currentUser);

      if (canViewAuditTrail) {
        await loadAuditLogs();
      }
    } catch (err) {
      console.error(err);
    }
  }

  function handleChecklistFieldChange(field, value) {
    setChecklistForm({
      ...checklistForm,
      [field]: value,
    });
  }

  function handleChecklistItemChange(index, field, value) {
    const updatedItems = checklistForm.items.map((item, itemIndex) => {
      if (itemIndex !== index) {
        return item;
      }

      return {
        ...item,
        [field]: field === "weight" ? Number(value) : value,
      };
    });

    setChecklistForm({
      ...checklistForm,
      items: updatedItems,
    });
  }

  function handleAddChecklistItem() {
    setChecklistForm({
      ...checklistForm,
      items: [...checklistForm.items, { ...emptyChecklistItem }],
    });
  }

  function handleRemoveChecklistItem(index) {
    if (checklistForm.items.length === 1) {
      setError("At least one checklist item is required.");
      return;
    }

    setChecklistForm({
      ...checklistForm,
      items: checklistForm.items.filter((_, itemIndex) => itemIndex !== index),
    });
  }

  async function handlePrintReport() {
    await logReportExport();
    window.print();
  }

  async function handleDownloadTextReport() {
    if (!selectedReview) {
      return;
    }

    await logReportExport();

    const reportContent = buildTextReport(selectedReview);
    const blob = new Blob([reportContent], { type: "text/plain;charset=utf-8" });
    const url = URL.createObjectURL(blob);
    const safeTitle = selectedReview.title
      .replace(/[^a-z0-9]/gi, "-")
      .replace(/-+/g, "-")
      .toLowerCase();

    const link = document.createElement("a");
    link.href = url;
    link.download = `${safeTitle || "policy-review"}-compliance-report.txt`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    URL.revokeObjectURL(url);
  }

  if (!currentUser) {
    return (
      <LoginPage
        loginForm={loginForm}
        setLoginForm={setLoginForm}
        handleLogin={handleLogin}
        loading={loading}
        error={error}
        successMessage={successMessage}
      />
    );
  }

  return (
    <div className="app-shell">
      <Header
        activePage={activePage}
        setActivePage={setActivePage}
        currentUser={currentUser}
        canCreateReviews={canCreateReviews}
        canViewAuditTrail={canViewAuditTrail}
        loadInitialData={loadInitialData}
        handleLogout={handleLogout}
        lastFullRefreshAt={lastFullRefreshAt}
      />

      <main className="main-content">
        {error && <div className="error-banner no-print">{error}</div>}

        {successMessage && (
          <div className="success-banner no-print">{successMessage}</div>
        )}

        {activePage === "dashboard" && (
          <DashboardPage
            dashboard={dashboard}
            auditLogs={auditLogs}
            canCreateReviews={canCreateReviews}
            setActivePage={setActivePage}
            handleViewReview={handleViewReview}
          />
        )}

        {activePage === "new-review" && canCreateReviews && (
          <NewReviewPage
            reviewForm={reviewForm}
            setReviewForm={setReviewForm}
            checklists={checklists}
            handleAnalyzePolicy={handleAnalyzePolicy}
            loading={loading}
          />
        )}

        {activePage === "saved-reviews" && (
          <SavedReviewsPage
            reviews={reviews}
            reviewFilter={reviewFilter}
            setReviewFilter={setReviewFilter}
            handleViewReview={handleViewReview}
            handleDeleteReview={handleDeleteReview}
            canDeleteReviews={canDeleteReviews}
            loading={loading}
          />
        )}

        {activePage === "checklists" && (
          <ChecklistsPage
            checklists={checklists}
            checklistForm={checklistForm}
            canManageChecklists={canManageChecklists}
            loading={loading}
            handleViewChecklist={handleViewChecklist}
            handleDeleteChecklist={handleDeleteChecklist}
            handleCreateChecklist={handleCreateChecklist}
            handleChecklistFieldChange={handleChecklistFieldChange}
            handleChecklistItemChange={handleChecklistItemChange}
            handleAddChecklistItem={handleAddChecklistItem}
            handleRemoveChecklistItem={handleRemoveChecklistItem}
          />
        )}

        {activePage === "checklist-detail" && (
          <ChecklistDetailPage
            selectedChecklist={selectedChecklist}
            setActivePage={setActivePage}
            canManageChecklists={canManageChecklists}
            handleDeleteChecklist={handleDeleteChecklist}
            loading={loading}
            isEditingChecklist={isEditingChecklist}
            setIsEditingChecklist={setIsEditingChecklist}
            editChecklistForm={editChecklistForm}
            handleEditChecklistFieldChange={handleEditChecklistFieldChange}
            handleEditChecklistItemChange={handleEditChecklistItemChange}
            handleAddEditChecklistItem={handleAddEditChecklistItem}
            handleRemoveEditChecklistItem={handleRemoveEditChecklistItem}
            handleStartEditChecklist={handleStartEditChecklist}
            handleCancelEditChecklist={handleCancelEditChecklist}
            handleUpdateChecklist={handleUpdateChecklist}
          />
        )}

        {activePage === "audit-trail" && canViewAuditTrail && (
          <AuditTrailPage
            auditLogs={auditLogs}
            auditFilter={auditFilter}
            setAuditFilter={setAuditFilter}
            loadAuditLogs={loadAuditLogs}
            lastAuditRefreshAt={lastAuditRefreshAt}
          />
        )}

        {activePage === "results" && selectedReview && (
          <ResultsPage
            selectedReview={selectedReview}
            setActivePage={setActivePage}
            canDeleteReviews={canDeleteReviews}
            handleDeleteReview={handleDeleteReview}
            loading={loading}
          />
        )}

        {activePage === "report" && selectedReview && (
          <ReportPage
            selectedReview={selectedReview}
            setActivePage={setActivePage}
            handlePrintReport={handlePrintReport}
            handleDownloadTextReport={handleDownloadTextReport}
          />
        )}
      </main>
    </div>
  );
}

export default App;