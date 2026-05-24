import {
  formatDateTime,
  getComplianceLevel,
  getScoreClass,
  getStatusClass,
} from "../utils/formatting";

function ResultsPage({
  selectedReview,
  setActivePage,
  canDeleteReviews,
  handleDeleteReview,
  loading,
}) {
  if (!selectedReview) {
    return null;
  }

  return (
    <section className="page-section">
      <div className="results-cover">
        <div>
          <p className="eyebrow">{selectedReview.checklist}</p>
          <h2>{selectedReview.title}</h2>
          <p>Reviewed on {formatDateTime(selectedReview.createdAt)}</p>
        </div>

        <div className="results-actions">
          <div
            className={`large-score ${getScoreClass(
              selectedReview.overallScore
            )}`}
          >
            <strong>{selectedReview.overallScore}%</strong>
            <span>{getComplianceLevel(selectedReview.overallScore)}</span>
          </div>

          <button
            className="primary-btn"
            onClick={() => setActivePage("report")}
          >
            Report View
          </button>

          {canDeleteReviews && (
            <button
              className="danger-btn"
              onClick={() => handleDeleteReview(selectedReview.id)}
              disabled={loading}
            >
              Delete Review
            </button>
          )}
        </div>
      </div>

      <div className="summary-strip">
        <div>
          <span>Passed</span>
          <strong>{selectedReview.summary?.passed ?? 0}</strong>
        </div>
        <div>
          <span>Needs Review</span>
          <strong>{selectedReview.summary?.needsReview ?? 0}</strong>
        </div>
        <div>
          <span>Missing</span>
          <strong>{selectedReview.summary?.missing ?? 0}</strong>
        </div>
        <div>
          <span>Total Items</span>
          <strong>{selectedReview.summary?.totalItems ?? 0}</strong>
        </div>
      </div>

      <div className="results-list">
        {selectedReview.results?.map((result) => (
          <div key={result.id || result.requirement} className="result-card">
            <div className="result-top">
              <div>
                <h4>{result.requirement}</h4>
                <p>{result.description}</p>
              </div>

              <span
                className={`status-pill ${getStatusClass(
                  result.resultStatus
                )}`}
              >
                {result.resultStatus}
              </span>
            </div>

            {result.matchedText && (
              <div className="matched-text">
                <strong>Matched:</strong> {result.matchedText}
              </div>
            )}

            <div className="recommendation">
              <strong>Recommendation:</strong> {result.recommendation}
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}

export default ResultsPage;