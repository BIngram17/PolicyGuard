import {
  formatDateOnly,
  getComplianceLevel,
  getScoreClass,
} from "../utils/formatting";

function DashboardPage({
  dashboard,
  auditLogs,
  canCreateReviews,
  setActivePage,
  handleViewReview,
}) {
  const recentReviews = dashboard?.recentReviews ?? [];

  const highComplianceCount = recentReviews.filter(
    (review) => review.overallScore >= 85
  ).length;

  const needsReviewCount = recentReviews.filter(
    (review) => review.overallScore >= 70 && review.overallScore < 85
  ).length;

  const atRiskCount = recentReviews.filter(
    (review) => review.overallScore < 70
  ).length;

  const totalRecentReviews = recentReviews.length;

  function getDistributionPercent(count) {
    if (totalRecentReviews === 0) {
      return 0;
    }

    return Math.round((count / totalRecentReviews) * 100);
  }

  const distributionRows = [
    {
      label: "High Compliance",
      count: highComplianceCount,
      percent: getDistributionPercent(highComplianceCount),
      className: "distribution-good",
    },
    {
      label: "Needs Review",
      count: needsReviewCount,
      percent: getDistributionPercent(needsReviewCount),
      className: "distribution-warning",
    },
    {
      label: "At Risk",
      count: atRiskCount,
      percent: getDistributionPercent(atRiskCount),
      className: "distribution-risk",
    },
  ];

  const strongestReview = [...recentReviews].sort(
    (firstReview, secondReview) =>
      secondReview.overallScore - firstReview.overallScore
  )[0];

  const riskiestReview = [...recentReviews].sort(
    (firstReview, secondReview) =>
      firstReview.overallScore - secondReview.overallScore
  )[0];

  return (
    <section className="dashboard-page">
      <div className="workspace-hero dashboard-hero-upgraded">
        <div>
          <p className="eyebrow">Compliance Overview</p>
          <h2>Policy review readiness at a glance</h2>
          <p>
            Track document reviews, identify missing compliance sections, and
            monitor policy quality across checklist templates.
          </p>
        </div>

        <div className="hero-score-card">
          <span>Average Score</span>
          <strong>{dashboard?.averageScore ?? 0}%</strong>
          <small>{getComplianceLevel(dashboard?.averageScore ?? 0)}</small>
        </div>
      </div>

      <div className="summary-strip dashboard-summary-strip">
        <div>
          <span>Total Reviews</span>
          <strong>{dashboard?.totalReviews ?? 0}</strong>
        </div>

        <div>
          <span>Needs Attention</span>
          <strong>{dashboard?.reviewsNeedingAttention ?? 0}</strong>
        </div>

        <div>
          <span>Checklist Templates</span>
          <strong>{dashboard?.totalChecklists ?? 0}</strong>
        </div>

        <div>
          <span>Audit Events</span>
          <strong>{auditLogs.length}</strong>
        </div>
      </div>

      <div className="dashboard-insight-grid">
        <section className="workspace-panel dashboard-chart-panel">
          <div className="panel-title-row">
            <div>
              <p className="eyebrow">Compliance Distribution</p>
              <h3>Recent review quality</h3>
              <span>
                Based on {totalRecentReviews} recent review
                {totalRecentReviews === 1 ? "" : "s"}
              </span>
            </div>
          </div>

          {totalRecentReviews > 0 ? (
            <div className="distribution-list">
              {distributionRows.map((row) => (
                <div key={row.label} className="distribution-row">
                  <div className="distribution-row-header">
                    <span>{row.label}</span>
                    <strong>
                      {row.count} review{row.count === 1 ? "" : "s"} ·{" "}
                      {row.percent}%
                    </strong>
                  </div>

                  <div className="distribution-track">
                    <div
                      className={`distribution-fill ${row.className}`}
                      style={{ width: `${row.percent}%` }}
                    ></div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p className="empty-text">
              Create policy reviews to populate the compliance distribution.
            </p>
          )}
        </section>

        <section className="workspace-panel dashboard-risk-panel">
          <p className="eyebrow">Risk Signal</p>
          <h3>Most common missing item</h3>

          {dashboard?.mostCommonMissingRequirement ? (
            <div className="risk-card upgraded-risk-card">
              <strong>{dashboard.mostCommonMissingRequirement.requirement}</strong>
              <span>
                Missing in {dashboard.mostCommonMissingRequirement.count} review
                {dashboard.mostCommonMissingRequirement.count === 1 ? "" : "s"}
              </span>
            </div>
          ) : (
            <p className="empty-text">
              No missing requirements have been identified yet.
            </p>
          )}

          {canCreateReviews && (
            <button
              className="primary-btn full-width"
              onClick={() => setActivePage("new-review")}
            >
              Start New Review
            </button>
          )}
        </section>
      </div>

      <div className="dashboard-grid">
        <section className="workspace-panel large-panel">
          <div className="panel-title-row">
            <div>
              <p className="eyebrow">Recent Activity</p>
              <h3>Latest policy reviews</h3>
            </div>

            <button onClick={() => setActivePage("saved-reviews")}>
              Review History
            </button>
          </div>

          {recentReviews.length > 0 ? (
            <div className="timeline-list">
              {recentReviews.map((review) => (
                <button
                  key={review.id}
                  className="timeline-item"
                  onClick={() => handleViewReview(review.id)}
                >
                  <div className="timeline-dot"></div>

                  <div>
                    <strong>{review.title}</strong>
                    <span>
                      {review.checklist} · {formatDateOnly(review.createdAt)}
                    </span>
                  </div>

                  <span
                    className={`score-pill ${getScoreClass(
                      review.overallScore
                    )}`}
                  >
                    {review.overallScore}%
                  </span>
                </button>
              ))}
            </div>
          ) : (
            <p className="empty-text">
              No reviews have been created yet. Start with a new review to
              populate the workspace.
            </p>
          )}
        </section>

        <section className="workspace-panel dashboard-score-panel">
          <p className="eyebrow">Review Highlights</p>
          <h3>Score snapshot</h3>

          {recentReviews.length > 0 ? (
            <div className="score-snapshot-list">
              <div className="score-snapshot-card">
                <span>Strongest Recent Review</span>
                <strong>{strongestReview?.title}</strong>
                <small
                  className={`score-pill ${getScoreClass(
                    strongestReview?.overallScore ?? 0
                  )}`}
                >
                  {strongestReview?.overallScore ?? 0}%
                </small>
              </div>

              <div className="score-snapshot-card">
                <span>Highest Risk Recent Review</span>
                <strong>{riskiestReview?.title}</strong>
                <small
                  className={`score-pill ${getScoreClass(
                    riskiestReview?.overallScore ?? 0
                  )}`}
                >
                  {riskiestReview?.overallScore ?? 0}%
                </small>
              </div>
            </div>
          ) : (
            <p className="empty-text">
              Review highlights will appear after policy reviews are created.
            </p>
          )}
        </section>
      </div>
    </section>
  );
}

export default DashboardPage;