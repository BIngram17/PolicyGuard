function ChecklistDetailPage({
  selectedChecklist,
  setActivePage,
  canManageChecklists,
  handleDeleteChecklist,
  loading,
  isEditingChecklist,
  setIsEditingChecklist,
  editChecklistForm,
  handleEditChecklistFieldChange,
  handleEditChecklistItemChange,
  handleAddEditChecklistItem,
  handleRemoveEditChecklistItem,
  handleStartEditChecklist,
  handleCancelEditChecklist,
  handleUpdateChecklist,
}) {
  if (!selectedChecklist) {
    return (
      <section className="page-section">
        <div className="page-heading">
          <p className="eyebrow">Checklist Template</p>
          <h2>No checklist selected</h2>
          <p>Select a checklist template to view its requirements.</p>
        </div>

        <button
          className="secondary-btn"
          onClick={() => setActivePage("checklists")}
        >
          Back to Checklists
        </button>
      </section>
    );
  }

  if (isEditingChecklist) {
    return (
      <section className="page-section">
        <div className="page-heading">
          <p className="eyebrow">Edit Checklist Template</p>
          <h2>{selectedChecklist.name}</h2>
          <p>
            Update the template details, requirements, analyzer keywords, and
            scoring weights.
          </p>
        </div>

        <div className="detail-actions no-print">
          <button className="secondary-btn" onClick={handleCancelEditChecklist}>
            Cancel Edit
          </button>

          <button
            className="secondary-btn"
            onClick={() => setActivePage("checklists")}
          >
            Back to Checklists
          </button>
        </div>

        <form className="workspace-panel checklist-form" onSubmit={handleUpdateChecklist}>
          <label>
            Checklist Name
            <input
              type="text"
              value={editChecklistForm.name}
              onChange={(event) =>
                handleEditChecklistFieldChange("name", event.target.value)
              }
            />
          </label>

          <label>
            Category
            <input
              type="text"
              value={editChecklistForm.category}
              onChange={(event) =>
                handleEditChecklistFieldChange("category", event.target.value)
              }
            />
          </label>

          <label>
            Description
            <textarea
              className="compact-textarea"
              value={editChecklistForm.description}
              onChange={(event) =>
                handleEditChecklistFieldChange("description", event.target.value)
              }
            />
          </label>

          <div className="checklist-items-editor">
            <div className="panel-title-row">
              <div>
                <h3>Checklist Items</h3>
                <span>{editChecklistForm.items.length} item(s)</span>
              </div>

              <button
                type="button"
                className="secondary-btn"
                onClick={handleAddEditChecklistItem}
              >
                Add Item
              </button>
            </div>

            {editChecklistForm.items.map((item, index) => (
              <div key={index} className="checklist-item-editor">
                <div className="item-editor-header">
                  <strong>Item {index + 1}</strong>

                  <button
                    type="button"
                    className="danger-btn"
                    onClick={() => handleRemoveEditChecklistItem(index)}
                  >
                    Remove
                  </button>
                </div>

                <label>
                  Requirement
                  <input
                    type="text"
                    value={item.requirement}
                    onChange={(event) =>
                      handleEditChecklistItemChange(
                        index,
                        "requirement",
                        event.target.value
                      )
                    }
                  />
                </label>

                <label>
                  Description
                  <textarea
                    className="compact-textarea"
                    value={item.description}
                    onChange={(event) =>
                      handleEditChecklistItemChange(
                        index,
                        "description",
                        event.target.value
                      )
                    }
                  />
                </label>

                <div className="form-row">
                  <label>
                    Keywords
                    <input
                      type="text"
                      value={item.keywords}
                      onChange={(event) =>
                        handleEditChecklistItemChange(
                          index,
                          "keywords",
                          event.target.value
                        )
                      }
                    />
                  </label>

                  <label>
                    Weight
                    <input
                      type="number"
                      min="1"
                      value={item.weight}
                      onChange={(event) =>
                        handleEditChecklistItemChange(
                          index,
                          "weight",
                          event.target.value
                        )
                      }
                    />
                  </label>
                </div>
              </div>
            ))}
          </div>

          <div className="form-actions">
            <button className="primary-btn" type="submit" disabled={loading}>
              {loading ? "Saving Changes..." : "Save Changes"}
            </button>

            <button
              type="button"
              className="secondary-btn"
              onClick={handleCancelEditChecklist}
            >
              Cancel
            </button>
          </div>
        </form>
      </section>
    );
  }

  return (
    <section className="page-section">
      <div className="page-heading">
        <p className="eyebrow">Checklist Template</p>
        <h2>{selectedChecklist.name}</h2>
        <p>{selectedChecklist.description}</p>
      </div>

      <div className="detail-actions no-print">
        <button
          className="secondary-btn"
          onClick={() => setActivePage("checklists")}
        >
          Back to Checklists
        </button>

        {canManageChecklists && (
          <>
            <button
              className="primary-btn"
              onClick={handleStartEditChecklist}
              disabled={loading}
            >
              Edit Template
            </button>

            <button
              className="danger-btn"
              onClick={() => handleDeleteChecklist(selectedChecklist.id)}
              disabled={loading}
            >
              Delete Template
            </button>
          </>
        )}
      </div>

      <div className="template-detail-grid">
        <div className="workspace-panel">
          <p className="eyebrow">Template Summary</p>

          <div className="template-meta-list">
            <div>
              <span>Name</span>
              <strong>{selectedChecklist.name}</strong>
            </div>

            <div>
              <span>Category</span>
              <strong>{selectedChecklist.category}</strong>
            </div>

            <div>
              <span>Total Items</span>
              <strong>{selectedChecklist.items?.length ?? 0}</strong>
            </div>
          </div>
        </div>

        <div className="workspace-panel">
          <p className="eyebrow">Purpose</p>
          <p className="template-description">{selectedChecklist.description}</p>
        </div>
      </div>

      <div className="workspace-panel">
        <div className="panel-title-row">
          <div>
            <p className="eyebrow">Checklist Requirements</p>
            <h3>Review rules used by the analyzer</h3>
            <span>{selectedChecklist.items?.length ?? 0} item(s)</span>
          </div>
        </div>

        <div className="template-items-list">
          {selectedChecklist.items?.map((item, index) => (
            <article key={item.id} className="template-item-card">
              <div className="template-item-header">
                <span>Requirement {index + 1}</span>
                <strong>Weight: {item.weight}</strong>
              </div>

              <h3>{item.requirement}</h3>
              <p>{item.description}</p>

              <div className="keyword-box">
                <strong>Analyzer Keywords</strong>
                <span>{item.keywords}</span>
              </div>
            </article>
          ))}
        </div>
      </div>
    </section>
  );
}

export default ChecklistDetailPage;