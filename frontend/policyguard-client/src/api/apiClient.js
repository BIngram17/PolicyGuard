import axios from "axios";

const DEFAULT_API_BASE_URL = "http://localhost:5069/api";

// Vite exposes browser-safe build-time environment variables through import.meta.env.
// In production, set VITE_API_BASE_URL to the deployed API URL, for example:
// https://policyguard-api.azurewebsites.net/api
export const API_BASE_URL = (
  import.meta.env.VITE_API_BASE_URL || DEFAULT_API_BASE_URL
).replace(/\/$/, "");

function getAuthHeaders(authData) {
  if (!authData?.token) {
    return {};
  }

  return {
    Authorization: `Bearer ${authData.token}`,
  };
}

export const apiClient = {
  login(credentials) {
    return axios.post(`${API_BASE_URL}/Auth/login`, credentials);
  },

  getDashboard(authData) {
    return axios.get(`${API_BASE_URL}/Dashboard/summary`, {
      headers: getAuthHeaders(authData),
    });
  },

  getChecklists(authData) {
    return axios.get(`${API_BASE_URL}/Checklists`, {
      headers: getAuthHeaders(authData),
    });
  },

  getChecklistById(checklistId, authData) {
    return axios.get(`${API_BASE_URL}/Checklists/${checklistId}`, {
      headers: getAuthHeaders(authData),
    });
  },

  createChecklist(payload, authData) {
    return axios.post(`${API_BASE_URL}/Checklists`, payload, {
      headers: getAuthHeaders(authData),
    });
  },

  updateChecklist(checklistId, payload, authData) {
    return axios.put(`${API_BASE_URL}/Checklists/${checklistId}`, payload, {
      headers: getAuthHeaders(authData),
    });
  },

  deleteChecklist(checklistId, authData) {
    return axios.delete(`${API_BASE_URL}/Checklists/${checklistId}`, {
      headers: getAuthHeaders(authData),
    });
  },

  getReviews(authData) {
    return axios.get(`${API_BASE_URL}/PolicyReviews`, {
      headers: getAuthHeaders(authData),
    });
  },

  getReviewById(reviewId, authData) {
    return axios.get(`${API_BASE_URL}/PolicyReviews/${reviewId}`, {
      headers: getAuthHeaders(authData),
    });
  },

  analyzePolicy(payload, authData) {
    return axios.post(`${API_BASE_URL}/PolicyReviews/analyze`, payload, {
      headers: getAuthHeaders(authData),
    });
  },

  deleteReview(reviewId, authData) {
    return axios.delete(`${API_BASE_URL}/PolicyReviews/${reviewId}`, {
      headers: getAuthHeaders(authData),
    });
  },

  logReportExport(reviewId, authData) {
    return axios.post(
      `${API_BASE_URL}/PolicyReviews/${reviewId}/audit-report-export`,
      {},
      {
        headers: getAuthHeaders(authData),
      }
    );
  },

  getAuditLogs(authData) {
    return axios.get(`${API_BASE_URL}/AuditLogs`, {
      headers: getAuthHeaders(authData),
    });
  },
};
