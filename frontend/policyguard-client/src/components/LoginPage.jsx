function LoginPage({
  loginForm,
  setLoginForm,
  handleLogin,
  loading,
  error,
  successMessage,
}) {
  return (
    <div className="login-page">
      <section className="login-card">
        <div className="login-brand">
          <div className="brand-mark">PG</div>
          <div>
            <h1>PolicyGuard</h1>
            <p>Enterprise compliance review workspace</p>
          </div>
        </div>

        <div className="login-intro">
          <p className="eyebrow">Secure Access</p>
          <h2>Sign in to continue</h2>
          <p>
            Enter your PolicyGuard account credentials to access compliance
            reviews, checklist templates, reports, and audit records.
          </p>
        </div>

        {error && <div className="error-banner">{error}</div>}
        {successMessage && <div className="success-banner">{successMessage}</div>}

        <form className="login-form" onSubmit={handleLogin}>
          <label>
            Email
            <input
              type="email"
              value={loginForm.email}
              onChange={(event) =>
                setLoginForm({ ...loginForm, email: event.target.value })
              }
            />
          </label>

          <label>
            Password
            <input
              type="password"
              value={loginForm.password}
              onChange={(event) =>
                setLoginForm({ ...loginForm, password: event.target.value })
              }
            />
          </label>

          <button className="primary-btn full-width" disabled={loading}>
            {loading ? "Signing In..." : "Sign In"}
          </button>
        </form>
      </section>
    </div>
  );
}

export default LoginPage;