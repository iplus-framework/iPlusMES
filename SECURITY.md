# Security Policy

## Supported versions

Only the latest release of the `main` branch is supported with security
fixes.

## Reporting a vulnerability

Please report vulnerabilities privately via GitHub's
**private vulnerability reporting** (Security tab → Report a vulnerability),
or by email to the address below if that is not possible.

https://github.com/iplus-framework/iPlusMES/security/advisories/new

**Contact:** damir.lisak@live.de (project maintainer)

Please include a description of the issue, the affected component
(e.g. `gip.mes.webservices`, a concrete BSO/AC component), the steps or
code needed to reproduce it, and your assessment of the impact. Do not
open a public issue for a security problem.

## Scope notes

- iPlusMES is a Manufacturing Execution System (MES) used to control
  production lines. Remotely reachable issues in server components
  (WCF/web services, database access, report/printing, and the
  framework-hosted MCP server on port 8750 / SQL tunnel on port 8751
  that also run inside the MES console) are treated with the highest
  priority.
- The underlying framework is a separate repository:
  github.com/iplus-framework/iPlus. If the issue is in the framework
  code, report it there; framework *interaction* issues (what MES passes
  into framework APIs) are in scope here.

## Response

We aim to acknowledge reports within 7 days and will work with you on a
coordinated fix and disclosure.
