import {
  formatDateTime,
  getComplianceLevel,
  getExecutiveSummary,
  getScoreClass,
  getStatusClass,
} from "../utils/formatting";

function ReportPage({
  selectedReview,
  setActivePage,
  handlePrintReport,
  handleDownloadTextReport,
}) {
  if (!selectedReview) {
    return null;
  }

  return (
    <section className="report-page">
      <div className="report-actions no-print">
        <button className="secondary-btn" onClick={() => setActivePage("results")}>
          Back to Results
        </button>

        <button className="primary-btn" onClick={handlePrintReport}>
          Print / Save as PDF
        </button>

        <button className="secondary-btn" onClick={handleDownloadTextReport}>
          Download Text Report
        </button>
      </div>

      <article className="compliance-report">
        <div className="report-cover">
          <div>
            <p className="report-label">PolicyGuard Compliance Report</p>
            <h1>{selectedReview.title}</h1>
            <p className="report-subtitle">
              Automated documentation review against the{" "}
              <strong>{selectedReview.checklist}</strong>.
            </p>
          </div>

          <div
            className={`report-score ${getScoreClass(
              selectedReview.overallScore
            )}`}
          >
            <span>{selectedReview.overallScore}%</span>
            <small>{getComplianceLevel(selectedReview.overallScore)}</small>
          </div>
        </div>

        <div className="report-meta-grid">
          <div>
            <span>Checklist</span>
            <strong>{selectedReview.checklist}</strong>
          </div>

          <div>
            <span>Status</span>
            <strong>{selectedReview.status}</strong>
          </div>

          <div>
            <span>Reviewed</span>
            <strong>{formatDateTime(selectedReview.createdAt)}</strong>
          </div>

          <div>
            <span>Review ID</span>
            <strong>#{selectedReview.id}</strong>
          </div>
        </div>

        <section className="report-section">
          <h2>Executive Summary</h2>
          <p>{getExecutiveSummary(selectedReview)}</p>
        </section>

        <section className="report-section">
          <h2>Compliance Summary</h2>

          <div className="report-summary-grid">
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
        </section>

        <section className="report-section">
          <h2>Detailed Findings</h2>

          <div className="report-findings">
            {selectedReview.results?.map((result, index) => (
              <div
                key={result.id || result.requirement}
                className="report-finding"
              >
                <div className="report-finding-header">
                  <div>
                    <span>Finding {index + 1}</span>
                    <h3>{result.requirement}</h3>
                  </div>

                  <span
                    className={`status-pill ${getStatusClass(
                      result.resultStatus
                    )}`}
                  >
                    {result.resultStatus}
                  </span>
                </div>

                <p>{result.description}</p>

                <div className="report-detail-grid">
                  <div>
                    <strong>Matched Text</strong>
                    <p>{result.matchedText || "No matching text found."}</p>
                  </div>

                  <div>
                    <strong>Score</strong>
                    <p>
                      {result.score} / {result.weight}
                    </p>
                  </div>
                </div>

                <div className="report-recommendation">
                  <strong>Recommendation</strong>
                  <p>{result.recommendation}</p>
                </div>
              </div>
            ))}
          </div>
        </section>

        <section className="report-section">
          <h2>Scoring Method</h2>
          <p>
            Passed items receive full credit based on the checklist item weight.
            Items marked Needs Review receive partial credit. Missing items
            receive zero points. The overall compliance score is calculated by
            dividing earned points by total available points.
          </p>
        </section>

        <footer className="report-footer">
          <p>Generated by PolicyGuard Compliance Analyzer</p>
          <p>C# · ASP.NET Core · SQL Server · Entity Framework Core · React</p>
        </footer>
      </article>
    </section>
  );
}

export default ReportPage;