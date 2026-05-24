function parseApiDateAsUtc(dateValue) {
  if (!dateValue) {
    return null;
  }

  if (dateValue instanceof Date) {
    return dateValue;
  }

  const dateText = String(dateValue);

  const hasTimezone =
    dateText.endsWith("Z") || /[+-]\d{2}:\d{2}$/.test(dateText);

  return new Date(hasTimezone ? dateText : `${dateText}Z`);
}

export function formatDateTime(dateValue) {
  const date = parseApiDateAsUtc(dateValue);

  if (!date || Number.isNaN(date.getTime())) {
    return "N/A";
  }

  return date.toLocaleString();
}

export function formatDateOnly(dateValue) {
  const date = parseApiDateAsUtc(dateValue);

  if (!date || Number.isNaN(date.getTime())) {
    return "N/A";
  }

  return date.toLocaleDateString();
}

export function getScoreClass(score) {
  if (score >= 85) return "score-good";
  if (score >= 70) return "score-warning";
  return "score-risk";
}

export function getStatusClass(status) {
  if (status === "Passed") return "status-passed";
  if (status === "Needs Review") return "status-review";
  return "status-missing";
}

export function getComplianceLevel(score) {
  if (score >= 85) return "High Compliance";
  if (score >= 70) return "Moderate Compliance";
  return "At Risk";
}

export function getActionLabel(action) {
  const labels = {
    POLICY_REVIEW_CREATED: "Policy Review Created",
    POLICY_REVIEW_DELETED: "Policy Review Deleted",
    CHECKLIST_CREATED: "Checklist Created",
    CHECKLIST_UPDATED: "Checklist Updated",
    CHECKLIST_DELETED: "Checklist Deleted",
    REPORT_EXPORTED: "Report Exported",
    LOGIN_SUCCESS: "Login Success",
    LOGIN_FAILED: "Login Failed",
    USER_REGISTERED: "User Registered",
  };

  return labels[action] || action;
}

export function getAuditActionClass(action) {
  if (action.includes("DELETED") || action.includes("FAILED")) {
    return "audit-danger";
  }

  if (action.includes("EXPORTED")) {
    return "audit-export";
  }

  if (action.includes("UPDATED")) {
    return "audit-update";
  }

  return "audit-create";
}

export function getExecutiveSummary(review) {
  if (!review) {
    return "";
  }

  const score = review.overallScore;
  const missing = review.summary?.missing ?? 0;
  const needsReview = review.summary?.needsReview ?? 0;

  if (score >= 85) {
    return "The reviewed document demonstrates strong compliance with the selected checklist. Most required sections appear to be addressed, although any items marked as missing or needing review should still be evaluated before final approval.";
  }

  if (score >= 70) {
    return "The reviewed document shows moderate compliance, but several items need additional review or clarification. The document may be usable as a draft, but it should be strengthened before being treated as final.";
  }

  return `The reviewed document is at risk because it is missing several required compliance elements. Before approval, the document should be revised to address ${missing} missing item(s) and ${needsReview} item(s) needing review.`;
}