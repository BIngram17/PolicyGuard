function ChecklistsPage({
  checklists,
  checklistForm,
  canManageChecklists,
  loading,
  handleViewChecklist,
  handleDeleteChecklist,
  handleCreateChecklist,
  handleChecklistFieldChange,
  handleChecklistItemChange,
  handleAddChecklistItem,
  handleRemoveChecklistItem,
}) {
  return (
    <section className="page-section">
      <div className="page-heading">
        <p className="eyebrow">Checklist Manager</p>
        <h2>Configure compliance rules</h2>
        <p>
          Create custom checklist templates that define the requirements,
          keywords, and scoring weights used by the analyzer.
        </p>
      </div>

      <div className="checklist-workspace">
        <section className="workspace-panel">
          <div className="panel-title-row">
            <div>
              <h3>Available Checklists</h3>
              <span>{checklists.length} template(s)</span>
            </div>
          </div>

          <div className="checklist-list">
            {checklists.map((checklist) => (
              <div key={checklist.id} className="checklist-card">
                <div>
                  <strong>{checklist.name}</strong>
                  <span>{checklist.category}</span>
                  <p>{checklist.description}</p>
                  <small>{checklist.itemCount} checklist item(s)</small>
                </div>

                <div className="checklist-actions">
                  <button
                    className="table-btn"
                    onClick={() => handleViewChecklist(checklist.id)}
                    disabled={loading}
                  >
                    Open Template
                  </button>

                  {canManageChecklists && (
                    <button
                      className="danger-btn"
                      onClick={() => handleDeleteChecklist(checklist.id)}
                      disabled={loading}
                    >
                      Delete
                    </button>
                  )}
                </div>
              </div>
            ))}
          </div>
        </section>

        {canManageChecklists && (
          <section className="workspace-panel">
            <div className="panel-title-row">
              <div>
                <p className="eyebrow">Create Template</p>
                <h3>New checklist</h3>
              </div>
            </div>

            <form className="checklist-form" onSubmit={handleCreateChecklist}>
              <label>
                Checklist Name
                <input
                  type="text"
                  placeholder="Example: Procurement Compliance Checklist"
                  value={checklistForm.name}
                  onChange={(event) =>
                    handleChecklistFieldChange("name", event.target.value)
                  }
                />
              </label>

              <label>
                Category
                <input
                  type="text"
                  placeholder="Example: Procurement"
                  value={checklistForm.category}
                  onChange={(event) =>
                    handleChecklistFieldChange("category", event.target.value)
                  }
                />
              </label>

              <label>
                Description
                <textarea
                  className="compact-textarea"
                  placeholder="Describe what this checklist reviews."
                  value={checklistForm.description}
                  onChange={(event) =>
                    handleChecklistFieldChange("description", event.target.value)
                  }
                />
              </label>

              <div className="checklist-items-editor">
                <div className="panel-title-row">
                  <div>
                    <h3>Checklist Items</h3>
                    <span>{checklistForm.items.length} item(s)</span>
                  </div>

                  <button
                    type="button"
                    className="secondary-btn"
                    onClick={handleAddChecklistItem}
                  >
                    Add Item
                  </button>
                </div>

                {checklistForm.items.map((item, index) => (
                  <div key={index} className="checklist-item-editor">
                    <div className="item-editor-header">
                      <strong>Item {index + 1}</strong>

                      <button
                        type="button"
                        className="danger-btn"
                        onClick={() => handleRemoveChecklistItem(index)}
                      >
                        Remove
                      </button>
                    </div>

                    <label>
                      Requirement
                      <input
                        type="text"
                        placeholder="Example: Approval Authority"
                        value={item.requirement}
                        onChange={(event) =>
                          handleChecklistItemChange(
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
                        placeholder="Explain what the document should include."
                        value={item.description}
                        onChange={(event) =>
                          handleChecklistItemChange(
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
                          placeholder="approved by, signoff, authorization"
                          value={item.keywords}
                          onChange={(event) =>
                            handleChecklistItemChange(
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
                            handleChecklistItemChange(
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

              <button
                className="primary-btn full-width"
                type="submit"
                disabled={loading}
              >
                {loading ? "Saving Checklist..." : "Create Checklist"}
              </button>
            </form>
          </section>
        )}
      </div>
    </section>
  );
}

export default ChecklistsPage;