import { useMemo, useState } from "react";
import {
  formatDateTime,
  getActionLabel,
  getAuditActionClass,
} from "../utils/formatting";

function AuditTrailPage({
  auditLogs,
  auditFilter,
  setAuditFilter,
  loadAuditLogs,
  lastAuditRefreshAt,
}) {
  const [searchTerm, setSearchTerm] = useState("");
  const [sortOption, setSortOption] = useState("newest");

  const uniqueAuditActions = useMemo(() => {
    return [...new Set(auditLogs.map((log) => log.action))];
  }, [auditLogs]);

  const filteredAuditLogs = useMemo(() => {
    let filtered = [...auditLogs];

    if (auditFilter !== "all") {
      filtered = filtered.filter((log) => log.action === auditFilter);
    }

    const normalizedSearch = searchTerm.trim().toLowerCase();

    if (normalizedSearch) {
      filtered = filtered.filter((log) => {
        const searchableText = [
          getActionLabel(log.action),
          log.action,
          log.summary,
          log.details,
          log.entityType,
          log.entityId?.toString(),
          log.performedBy,
          formatDateTime(log.createdAt),
        ]
          .filter(Boolean)
          .join(" ")
          .toLowerCase();

        return searchableText.includes(normalizedSearch);
      });
    }

    return filtered.sort((firstLog, secondLog) => {
      if (sortOption === "oldest") {
        return new Date(firstLog.createdAt) - new Date(secondLog.createdAt);
      }

      if (sortOption === "action-az") {
        return getActionLabel(firstLog.action).localeCompare(
          getActionLabel(secondLog.action)
        );
      }

      if (sortOption === "user-az") {
        return (firstLog.performedBy || "").localeCompare(
          secondLog.performedBy || ""
        );
      }

      if (sortOption === "entity-az") {
        return (firstLog.entityType || "").localeCompare(
          secondLog.entityType || ""
        );
      }

      return new Date(secondLog.createdAt) - new Date(firstLog.createdAt);
    });
  }, [auditLogs, auditFilter, searchTerm, sortOption]);

  return (
    <section className="page-section">
      <div className="page-heading">
        <p className="eyebrow">Audit Trail</p>
        <h2>System activity and compliance events</h2>
        <p>
          Review system-generated records for policy reviews, checklist changes,
          deleted records, login activity, and exported reports.
        </p>
      </div>

      <div className="audit-summary-grid">
        <div>
          <span>Total Events</span>
          <strong>{auditLogs.length}</strong>
        </div>

        <div>
          <span>Review Events</span>
          <strong>
            {auditLogs.filter((log) => log.entityType === "PolicyReview").length}
          </strong>
        </div>

        <div>
          <span>Checklist Events</span>
          <strong>
            {
              auditLogs.filter(
                (log) => log.entityType === "ComplianceChecklist"
              ).length
            }
          </strong>
        </div>

        <div>
          <span>Exports</span>
          <strong>
            {auditLogs.filter((log) => log.action === "REPORT_EXPORTED").length}
          </strong>
        </div>
      </div>

      <div className="workspace-panel">
        <div className="panel-title-row">
          <div>
            <h3>Audit Events</h3>
            <span>{filteredAuditLogs.length} visible event(s)</span>

            {lastAuditRefreshAt && (
              <small className="page-refresh-timestamp">
                Last refreshed: {formatDateTime(lastAuditRefreshAt)}
              </small>
            )}
          </div>

          <button className="secondary-btn" onClick={loadAuditLogs}>
            Refresh Logs
          </button>
        </div>

        <div className="audit-toolbar">
          <div className="audit-controls-grid">
            <label className="audit-search">
              Search Audit Logs
              <input
                type="search"
                placeholder="Search by action, user, entity, details, date..."
                value={searchTerm}
                onChange={(event) => setSearchTerm(event.target.value)}
              />
            </label>

            <label className="audit-sort">
              Sort Audit Logs
              <select
                value={sortOption}
                onChange={(event) => setSortOption(event.target.value)}
              >
                <option value="newest">Newest First</option>
                <option value="oldest">Oldest First</option>
                <option value="action-az">Action A-Z</option>
                <option value="user-az">User A-Z</option>
                <option value="entity-az">Entity A-Z</option>
              </select>
            </label>
          </div>

          <div className="filter-bar">
            <button
              className={auditFilter === "all" ? "active-filter" : ""}
              onClick={() => setAuditFilter("all")}
            >
              All
            </button>

            {uniqueAuditActions.map((action) => (
              <button
                key={action}
                className={auditFilter === action ? "active-filter" : ""}
                onClick={() => setAuditFilter(action)}
              >
                {getActionLabel(action)}
              </button>
            ))}
          </div>
        </div>

        {filteredAuditLogs.length > 0 ? (
          <div className="audit-list">
            {filteredAuditLogs.map((log) => (
              <article key={log.id} className="audit-card">
                <div className="audit-card-top">
                  <span
                    className={`audit-badge ${getAuditActionClass(log.action)}`}
                  >
                    {getActionLabel(log.action)}
                  </span>

                  <time>{formatDateTime(log.createdAt)}</time>
                </div>

                <h3>{log.summary}</h3>

                <div className="audit-meta">
                  <span>Entity: {log.entityType}</span>
                  <span>ID: {log.entityId ?? "N/A"}</span>
                  <span>By: {log.performedBy}</span>
                </div>

                {log.details && <p>{log.details}</p>}
              </article>
            ))}
          </div>
        ) : (
          <p className="empty-text">
            No audit events match the current search or filter.
          </p>
        )}
      </div>
    </section>
  );
}

export default AuditTrailPage;