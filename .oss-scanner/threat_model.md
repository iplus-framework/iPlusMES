# Threat model — iPlusMES (iplus-framework)

## What this project is

iPlusMES is the Manufacturing Execution System layer of the iplus-framework
(GPL-3.0-or-later, .NET 10, C#). It runs inside plants on top of the iPlus
framework (the repository `iplus-framework/iPlus`, which this build clones next
to the checkout because all MES projects reference the framework sources). It
controls production processes, order/shop-floor scheduling and machine data —
vulnerabilities can have physical-world impact.

## Where untrusted input enters

- The WCF/web services of the MES business objects (`gip.mes.webservices`) and
  of the framework below them. Remote, sometimes anonymous.
- MES business logic endpoints reachable from shop-floor clients
  (`gip.bso.*`, `gip.mes.*`), e.g. production data, order handling, logistics.
- SQL injection through dynamic queries, reports and script generation
  (`gip.mes.sql`, `gip.mes.datamodel`, report handlers).
- XML / file deserialization: MES process applications, IPCDrives/process
  configuration, imported/archived files (`gip.mes.archiver`).
- MES client (`gip.mes.client*`) rendering of server data in the browser client.
- Framework-hosted service endpoints that also run inside the MES runtime
  host (`gip.mes.console` starts the full framework stack): the MCP Server
  (`PAMcpServerHost`, port 8750) and the SQL Tunnel (`PASqlTunnelHost`,
  port 8751) are audited at the framework level (`projects/iplus`), but MES
  tools/prompts, MES business-object methods reachable through them, and
  any MES-side configuration that weakens their authentication are in
  scope here.

## Components that matter most / least

Most important: `gip.mes.webservices`, `gip.mes.datamodel`, `gip.mes.sql`,
`gip.mes.processapplication`, `gip.mes.autocomponent`, `gip.mes.console`
(the enrolled runtime host), and the BSO layers `gip.bso.*`
(manufacturing, masterdata, logistics, facility, purchasing, sales).

Less important but in scope: `gip.mes.client*` UI projects,
`gip.mes.cmdlet`/`gip2006.variobatch.processapplication`.

Out of scope:

- The iPlus framework repository itself — it is enrolled as its own project
  (`projects/iplus`); duplicate reports on framework code are unnecessary but
  framework *interaction* issues (what MES passes into framework APIs) are in scope.
- Vendored/forked third-party libraries under the framework's `gip.ext.*`.

## How to exercise it

- `.oss-scanner/run-console.sh` starts a local SQL Server (pre-installed in the
  image), restores the IPCDrivesV5 demo database from
  `Database/IPCDrivesV5.7z` and runs the headless MES console
  `gip.mes.console` (login user `00` / password `00`) as a service against
  it. Both entity connection strings point at the IPCDrivesV5 catalog. The
  console runs on top of the cloned iPlus framework checkout (`/iPlus`);
  the console build output is `/src/bin/Release/net10.0`.
- The framework's agent documentation (`/iPlus/gip.iplus.console/readme.md`)
  also applies: the console hosts WCF/web services and stops gracefully on
  SIGTERM.
- The image needs at least ~2 GB of RAM because SQL Server runs inside it.

## How you rate severity

- Critical: unauthenticated remote code execution (XML deserialization, script
  execution, report engines), authentication bypass, arbitrary machine/process
  control through the MES application model.
- High: SQL injection reachable through business endpoints, unsafe
  deserialization of untrusted XML, path traversal / arbitrary file access,
  privilege escalation between shop-floor users (e.g. one user reading or
  writing another's orders).
- Medium: authenticated information disclosure, SSRF, DoS by crashing a
  service host, cross-tenant/machine data injection.
- Low: client-side issues requiring a malicious local user.

## Anything to leave alone

- Do not report dev credentials in committed config files (`gip`/`netspirit`,
  `00`/`00`) — sample/dev values. A hardcoded production credential in product
  code is still in scope.
- Do not report `Encrypt=False` in sample connection strings or self-signed
  development certificates.
- Do not report style/performance issues; vulnerabilities only.
