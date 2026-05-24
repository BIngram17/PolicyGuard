import { formatDateTime } from "../utils/formatting";

function Header({
  activePage,
  setActivePage,
  currentUser,
  canCreateReviews,
  canViewAuditTrail,
  loadInitialData,
  handleLogout,
  lastFullRefreshAt,
}) {
  return (
    <header className="workspace-header no-print">
      <div className="header-inner">
        <div className="brand-lockup">
          <div className="brand-mark">PG</div>
          <div>
            <h1>PolicyGuard</h1>
            <p>Enterprise compliance review workspace</p>
          </div>
        </div>

        <nav className="workspace-nav">
          <button
            className={activePage === "dashboard" ? "active" : ""}
            onClick={() => setActivePage("dashboard")}
          >
            Dashboard
          </button>

          {canCreateReviews && (
            <button
              className={activePage === "new-review" ? "active" : ""}
              onClick={() => setActivePage("new-review")}
            >
              New Review
            </button>
          )}

          <button
            className={activePage === "saved-reviews" ? "active" : ""}
            onClick={() => setActivePage("saved-reviews")}
          >
            Saved Reviews
          </button>

          <button
            className={activePage === "checklists" ? "active" : ""}
            onClick={() => setActivePage("checklists")}
          >
            Checklists
          </button>

          {canViewAuditTrail && (
            <button
              className={activePage === "audit-trail" ? "active" : ""}
              onClick={() => setActivePage("audit-trail")}
            >
              Audit Trail
            </button>
          )}
        </nav>

        <div className="user-menu">
          <div>
            <strong>{currentUser.fullName}</strong>
            <span>{currentUser.role}</span>
            {lastFullRefreshAt && (
              <small className="refresh-timestamp">
                Last refreshed: {formatDateTime(lastFullRefreshAt)}
              </small>
            )}
          </div>

          <button className="refresh-btn" onClick={() => loadInitialData()}>
            Refresh
          </button>

          <button className="secondary-btn" onClick={handleLogout}>
            Logout
          </button>
        </div>
      </div>
    </header>
  );
}

export default Header;