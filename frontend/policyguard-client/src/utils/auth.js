export const AUTH_STORAGE_KEY = "policyguard_auth";

export function getSavedAuth() {
  const savedAuth = localStorage.getItem(AUTH_STORAGE_KEY);

  if (!savedAuth) {
    return null;
  }

  try {
    return JSON.parse(savedAuth);
  } catch {
    localStorage.removeItem(AUTH_STORAGE_KEY);
    return null;
  }
}

export function saveAuth(authData) {
  localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(authData));
}

export function clearAuth() {
  localStorage.removeItem(AUTH_STORAGE_KEY);
}