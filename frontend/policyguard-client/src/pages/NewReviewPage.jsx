function NewReviewPage({
  reviewForm,
  setReviewForm,
  checklists,
  handleAnalyzePolicy,
  loading,
}) {
  return (
    <section className="page-section">
      <div className="page-heading">
        <p className="eyebrow">New Compliance Review</p>
        <h2>Analyze a policy or procedure</h2>
        <p>
          Paste document text, select a checklist, and generate a structured
          compliance review.
        </p>
      </div>

      <form className="review-form" onSubmit={handleAnalyzePolicy}>
        <div className="form-row">
          <label>
            Review Title
            <input
              type="text"
              placeholder="Example: Remote Access Policy Review"
              value={reviewForm.title}
              onChange={(event) =>
                setReviewForm({ ...reviewForm, title: event.target.value })
              }
            />
          </label>

          <label>
            Checklist
            <select
              value={reviewForm.complianceChecklistId}
              onChange={(event) =>
                setReviewForm({
                  ...reviewForm,
                  complianceChecklistId: event.target.value,
                })
              }
            >
              <option value="">Select checklist</option>
              {checklists.map((checklist) => (
                <option key={checklist.id} value={checklist.id}>
                  {checklist.name}
                </option>
              ))}
            </select>
          </label>
        </div>

        <label>
          Policy or Procedure Text
          <textarea
            placeholder="Paste policy, procedure, SDLC, or change management text here..."
            value={reviewForm.documentText}
            onChange={(event) =>
              setReviewForm({
                ...reviewForm,
                documentText: event.target.value,
              })
            }
          />
        </label>

        <div className="form-actions">
          <button type="submit" className="primary-btn" disabled={loading}>
            {loading ? "Analyzing..." : "Analyze Policy"}
          </button>
        </div>
      </form>
    </section>
  );
}

export default NewReviewPage;