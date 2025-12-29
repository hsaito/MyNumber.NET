# Security Policy

Thank you for helping keep MyNumber.NET and its users safe.

This repository contains multiple components:
- MyNumberNET (library)
- MyNumberNET_ApiServer (Web API)
- MyNumberNET_CLI (command-line tool)

Security guidance applies to all components unless stated otherwise.

## Supported Versions

Security fixes are provided for maintained versions on a best-effort basis.

| Version/Branch | Status |
| --- | --- |
| main | Actively developed; receives security fixes |
| Latest release | Supported while not superseded by a newer release |
| Older releases | Best-effort only; consider upgrading |

Refer to the GitHub Releases page for specific version numbers.

## Reporting a Vulnerability

Please do not open public GitHub issues for security problems.

- Preferred: Use GitHub Security Advisories to report privately to maintainers.
  - Repository → Security → Report a vulnerability
  - https://github.com/hsaito/MyNumber.NET/security/advisories/new
- If Security Advisories are unavailable for you, open a new issue marked clearly as "Security" and request conversion to a private advisory. Do not include exploit details or sensitive information in a public issue.

When reporting, include:
- Affected component (library/API/CLI) and version or commit
- Environment (OS, .NET runtime), configuration, and dependencies
- Impact assessment (confidentiality/integrity/availability)
- Reproduction steps, minimal proof-of-concept, and any mitigations
- References (CVE/CWE) if known

We appreciate responsible disclosure and will work with you to verify, remediate, and coordinate a fix.

## Coordinated Disclosure Policy

- Acknowledgement: We aim to acknowledge reports within 3 business days.
- Triage: We assess severity and scope as quickly as feasible.
- Remediation: We strive to develop and test fixes promptly; timelines depend on complexity and risk.
- Release: Fixes are released in the main branch and latest supported releases. We may issue advisories or release notes describing the impact and mitigation.
- Credit: With your consent, we will credit reporters in release notes/advisories.

## Security Updates & Dependencies

- Automated updates: Dependabot is used to help keep dependencies current (see related repository activity/PRs). This reduces exposure to known vulnerabilities.
- Review: Dependency bumps are reviewed and tested before merge where practical.
- Pinning: Critical dependencies may be pinned or constrained to safe versions.
- Supply chain: We prefer well-maintained packages and monitor for deprecations.

## Secure Development & Scanning

- Best-effort scanning: We aim to use security scanning tools (e.g., Snyk) for code and dependency analysis where applicable. Findings are triaged and addressed based on severity.
- Practices: We avoid introducing secrets in code, prefer least-privilege defaults, and review changes with security in mind.

## Hardening Guidance (API Server)

For deployments of `MyNumberNET_ApiServer`:
- Run behind HTTPS with modern TLS.
- Store secrets (API keys, connection strings) outside source control and rotate regularly.
- Restrict network access; place behind a firewall or reverse proxy.
- Keep the runtime and OS patched.
- Enable structured logging and monitor for anomalies.
- Validate inputs and enforce authentication/authorization at the API boundary.

## Scope & Non-Goals

- This project does not currently operate a public bug bounty program.
- We do not guarantee response times or fixes, but will act in good faith and prioritize high-severity issues.

## Contact & Questions

For general security questions (non-sensitive), open a discussion or issue and prefix the title with "Security". For sensitive reports, please use GitHub Security Advisories as described above.

We value the community’s help in identifying and fixing vulnerabilities. Thank you for contributing to a safer ecosystem.
