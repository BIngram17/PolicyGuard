function LoginPage({
  loginForm,
  setLoginForm,
  handleLogin,
  loading,
  error,
  successMessage,
  isDemoLink,
}) {
  const demoEmail = import.meta.env.VITE_DEMO_EMAIL?.trim();

  const useDemoEmail = () => {
    setLoginForm({ ...loginForm, email: demoEmail });
  };

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

        {isDemoLink ? (
          <aside className="demo-access-card demo-link-ready">
            <div>
              <p className="eyebrow">Resume Demo</p>
              <h3>Your Reviewer access is ready</h3>
              <p>
                The credentials are prepared for this portfolio session. Press
                Sign In to explore the working application with
                least-privilege Reviewer access.
              </p>
            </div>
          </aside>
        ) : demoEmail ? (
          <aside className="demo-access-card">
            <div>
              <p className="eyebrow">Portfolio Demo</p>
              <h3>Recruiter and hiring manager access</h3>
              <p>
                Use the shared demo password with the Reviewer account below.
                This role can explore reviews, run an analysis, and generate
                reports without administrative access.
              </p>
            </div>
            <button type="button" onClick={useDemoEmail}>
              Use {demoEmail}
            </button>
          </aside>
        ) : null}

        {error && <div className="error-banner">{error}</div>}
        {successMessage && <div className="success-banner">{successMessage}</div>}

        <form className="login-form" onSubmit={handleLogin}>
          <label>
            Email
            <input
              type="email"
              autoComplete="username"
              required
              readOnly={isDemoLink}
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
              autoComplete="current-password"
              required
              readOnly={isDemoLink}
              value={loginForm.password}
              onChange={(event) =>
                setLoginForm({ ...loginForm, password: event.target.value })
              }
            />
          </label>

          <button className="primary-btn full-width" disabled={loading}>
            {loading
              ? "Signing In..."
              : isDemoLink
                ? "Sign In to Live Demo"
                : "Sign In"}
          </button>
        </form>
      </section>
    </div>
  );
}

export default LoginPage;
