import { useMemo, useState } from "react";
import { formatDateOnly, getScoreClass } from "../utils/formatting";

function SavedReviewsPage({
  reviews,
  reviewFilter,
  setReviewFilter,
  handleViewReview,
  handleDeleteReview,
  canDeleteReviews,
  loading,
}) {
  const [searchTerm, setSearchTerm] = useState("");
  const [sortOption, setSortOption] = useState("newest");

  const filteredReviews = useMemo(() => {
    let filtered = [...reviews];

    if (reviewFilter === "high") {
      filtered = filtered.filter((review) => review.overallScore >= 85);
    }

    if (reviewFilter === "review") {
      filtered = filtered.filter(
        (review) => review.overallScore >= 70 && review.overallScore < 85
      );
    }

    if (reviewFilter === "risk") {
      filtered = filtered.filter((review) => review.overallScore < 70);
    }

    const normalizedSearch = searchTerm.trim().toLowerCase();

    if (normalizedSearch) {
      filtered = filtered.filter((review) => {
        const searchableText = [
          review.title,
          review.checklist,
          review.status,
          review.overallScore?.toString(),
          formatDateOnly(review.createdAt),
        ]
          .filter(Boolean)
          .join(" ")
          .toLowerCase();

        return searchableText.includes(normalizedSearch);
      });
    }

    return filtered.sort((firstReview, secondReview) => {
      if (sortOption === "oldest") {
        return new Date(firstReview.createdAt) - new Date(secondReview.createdAt);
      }

      if (sortOption === "highest-score") {
        return secondReview.overallScore - firstReview.overallScore;
      }

      if (sortOption === "lowest-score") {
        return firstReview.overallScore - secondReview.overallScore;
      }

      if (sortOption === "title-az") {
        return firstReview.title.localeCompare(secondReview.title);
      }

      return new Date(secondReview.createdAt) - new Date(firstReview.createdAt);
    });
  }, [reviews, reviewFilter, searchTerm, sortOption]);

  return (
    <section className="page-section">
      <div className="page-heading">
        <p className="eyebrow">Review Archive</p>
        <h2>Saved compliance reviews</h2>
        <p>
          Search, filter, sort, inspect, export, or remove previously analyzed
          policy reviews.
        </p>
      </div>

      <div className="workspace-panel">
        <div className="panel-title-row">
          <div>
            <h3>Review History</h3>
            <span>{filteredReviews.length} visible review(s)</span>
          </div>
        </div>

        <div className="archive-toolbar">
          <div className="archive-controls-grid">
            <label className="archive-search">
              Search Reviews
              <input
                type="search"
                placeholder="Search by title, checklist, status, score..."
                value={searchTerm}
                onChange={(event) => setSearchTerm(event.target.value)}
              />
            </label>

            <label className="archive-sort">
              Sort Reviews
              <select
                value={sortOption}
                onChange={(event) => setSortOption(event.target.value)}
              >
                <option value="newest">Newest First</option>
                <option value="oldest">Oldest First</option>
                <option value="highest-score">Highest Score</option>
                <option value="lowest-score">Lowest Score</option>
                <option value="title-az">Title A-Z</option>
              </select>
            </label>
          </div>

          <div className="filter-bar">
            <button
              className={reviewFilter === "all" ? "active-filter" : ""}
              onClick={() => setReviewFilter("all")}
            >
              All
            </button>

            <button
              className={reviewFilter === "high" ? "active-filter" : ""}
              onClick={() => setReviewFilter("high")}
            >
              High Compliance
            </button>

            <button
              className={reviewFilter === "review" ? "active-filter" : ""}
              onClick={() => setReviewFilter("review")}
            >
              Needs Review
            </button>

            <button
              className={reviewFilter === "risk" ? "active-filter" : ""}
              onClick={() => setReviewFilter("risk")}
            >
              At Risk
            </button>
          </div>
        </div>

        {filteredReviews.length > 0 ? (
          <table className="reviews-table">
            <thead>
              <tr>
                <th>Title</th>
                <th>Checklist</th>
                <th>Score</th>
                <th>Status</th>
                <th>Created</th>
                <th></th>
              </tr>
            </thead>

            <tbody>
              {filteredReviews.map((review) => (
                <tr key={review.id}>
                  <td>{review.title}</td>
                  <td>{review.checklist}</td>
                  <td>
                    <span
                      className={`score-pill ${getScoreClass(
                        review.overallScore
                      )}`}
                    >
                      {review.overallScore}%
                    </span>
                  </td>
                  <td>{review.status}</td>
                  <td>{formatDateOnly(review.createdAt)}</td>
                  <td>
                    <div className="table-actions">
                      <button
                        className="table-btn"
                        onClick={() => handleViewReview(review.id)}
                        disabled={loading}
                      >
                        View
                      </button>

                      {canDeleteReviews && (
                        <button
                          className="danger-btn"
                          onClick={() => handleDeleteReview(review.id)}
                          disabled={loading}
                        >
                          Delete
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p className="empty-text">
            No saved reviews match the current search or filter.
          </p>
        )}
      </div>
    </section>
  );
}

export default SavedReviewsPage;