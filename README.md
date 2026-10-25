# iPlus-MES

**iPlus-MES is a highly configurable, extendable and adaptable Manufacturing Execution System (MES)**
that closes the gap between enterprise-level systems (ERP) and field-level operations. Built on the
[iPlus-framework](https://github.com/iplus-framework/iPlus), it combines SCADA and MES in a single
application — so operators see machines and business objects in one consistent interface.

![iPlus-MES screenshot](https://github.com/iplus-framework/iPlus-Docs/blob/main/Images/iplus-MES-Screenshot.jpg)

## Table of Contents

- [Highlights](#highlights)
- [Features](#features)
- [Why choose iPlus-MES?](#why-choose-iplus-mes)
- [Technology](#technology)
- [Getting Started](#getting-started)
- [Contributing](#contributing)
- [Entity Framework Core](#entity-framework-core)
- [Documentation](#documentation)
- [License](#license)
- [Contact](#contact)

## Highlights

- 🖥️ **One UI, every platform** — Avalonia UI runs on Windows, Linux, macOS, Android and in the
  browser. The same designs, no separate code base.
- 🏭 **SCADA + MES merged** — navigate seamlessly between physical objects (machines, modules) and
  business objects (orders, batches, materials) without keyboard entry.
- 🧱 **Normalized, adaptable data model** — the model adapts to your production and IT landscape,
  not the other way round.
- 🔌 **Industry-agnostic** — extend the standard with sector-specific add-ons, or build your own
  solution on top of it.
- 🔄 **Updateable in production** — extend and adapt while the system is running, without losing
  updateability.
- 📈 **Proven in the field** — 30+ installations, 500+ workplaces/users, 4000+ machines and devices.

## Features

- **Stability and performance** — optimal operation and user-friendly presentation of complex
  production processes.
- **Architecture** — a normalized database model that adapts to existing production and IT systems.
- **Industry agnostic** — suitable for many sectors thanks to its customizable structure, with a
  focus on the process industry.
- **DCS** — distributed control system.
- **Usability** — an intuitive graphical user interface that merges SCADA and MES functionality.
- **Comprehensive modules:**
  - Master data management
  - Production and control
  - Material management
  - Logistics
  - Quality management
  - Maintenance
- **Real openness and extendability** — expand as required: by inheriting existing components, by
  developing additional components, or by using iPlus packages that contain industry solutions.

## Why choose iPlus-MES?

- **Resource utilization** — helps achieve optimal resource utilization and high efficiency.
- **Transparency** — renders all processes considerably more transparent.
- **Proven success** — 30+ installations, 500+ workplaces/users, 4000+ machines and devices.

## Technology

- **Runtime:** .NET 10 — one code base for server, desktop, mobile and browser.
- **Server side (no UI):** Windows and Linux. Runs as a console host or as a Windows/Linux service.
- **Client side (UI):** **Avalonia UI** is the primary UI technology and is fully functional on:
  - Windows (x64)
  - Linux (x64)
  - macOS
  - Android
  - Web browser (WebAssembly — nothing to install)

  The framework also ships an iOS head project; an iPlus-MES iOS head can be added the same way.
- **Data access:** Entity Framework Core, using our own optimized `iPlus.EntityFrameworkCore.*`
  packages (see [Entity Framework Core](#entity-framework-core)).
- **Communication:** network-transparent RMI, WCF/CoreWCF, OPC-UA client and server, PLC drivers.
- **UI definition:** XAML stored in the database and rendered at runtime — the same design runs on
  desktop, mobile and browser without rework.

> The WPF client remains in the repository for Windows-only and legacy deployments, but Avalonia UI
> is the strategic UI platform and the one used for all cross-platform releases.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A Microsoft SQL Server edition (Express, Developer or Standard) — on Windows or Linux, or in Docker
- Optional: Visual Studio 2022+, JetBrains Rider or VS Code

### 1. Clone both repositories

iPlus-MES builds on the framework, so both repositories must sit **side by side** in the same
parent directory — the project files use relative paths.

```bash
git clone https://github.com/iplus-framework/iPlus.git
git clone https://github.com/iplus-framework/iPlusMES.git
```

### 2. Install SQL Server

Follow the [Microsoft SQL Server installation guide](https://learn.microsoft.com/en-us/sql/database-engine/install-windows/install-sql-server?view=sql-server-ver16).

### 3. Restore a MES database

Restore the SQL Server backup file located in the `Database` folder:

- **`IPCDrivesV5`** — a fully configured demo plant. Best for exploring the features.
- **`iplusMESV5`** — an empty MES to start building your own from scratch.

### 4. Configure the connection string

Edit `ConnectionStrings.config` in the project you want to start. The exact steps are described in
[gipSoft's documentation](https://iplus-framework.com/en/documentation/Read/Index/View/b00675a8-718c-4c13-9d6d-5e751397ac5f?chapterID=193d292e-df31-405e-a3e9-f1116846bf86#ItemTextTranslationID_d0551cc7-f767-4790-8ecb-8771836ebac7).

### 5. Build and run

Pick the client that matches your target platform. All of them share the same code and designs.

```bash
# Desktop (Windows, Linux, macOS)
dotnet run --project gip.mes.client.avui.Desktop

# Browser (WebAssembly)
dotnet run --project gip.mes.client.avui.Browser

# Android
dotnet run --project gip.mes.client.avui.Android

# Server service without UI
dotnet run --project gip.mes.console -- /U00 /P00
```

> **No WINE required.** The Avalonia UI client runs natively on Linux and macOS. WINE is only
> needed if you want to run the legacy WPF client on Linux.

### 6. Log in

- **`superuser` / `superuser`** — start as a **client**.
- **`00` / `00`** — start as a **service**.

After starting the service, your local IP is remembered; start a second instance and log on as a
client to establish the client-server connection. If the client cannot connect, read the
[network configuration section](https://iplus-framework.com/en/documentation/Read/Index/View/b00675a8-718c-4c13-9d6d-5e751397ac5f?chapterID=a7424f03-cbff-4f68-90eb-262b3fdcba1c#ItemTextTranslationID_bf788469-8a2f-4a7c-ad78-2710960096dc).

> **Important:** every time you pull a newer version from GitHub, **hold down the CTRL key** while
> clicking the [login button](https://iplus-framework.com/en/documentation/Read/Index/View/b00675a8-718c-4c13-9d6d-5e751397ac5f?chapterID=ca1a96f0-233d-4e6c-9cfc-d1db683e4be3#ItemTextTranslationID_e24a12c4-649d-4903-abf0-8366ed656ca1)
> so that your local databases are updated. Otherwise the application will not start.

🤗 Have fun trying it out and watch our videos to get started. We also recommend
[online training](https://iplus-framework.com/en?section=Support%20%26%20Training#d57ccb45-9050-41cb-a177-9e8c05028931).

## Contributing

We welcome contributions from the community! 

Are you passionate about open-source development and eager to make a real impact in the world of industrial software? The gipSoft is looking for talented developers like you to help us push the boundaries of innovation and autonomy in software solutions.

By contributing to the iPlus-framework, you'll be part of a dynamic community dedicated to creating flexible, scalable, and efficient software that empowers businesses to thrive in the era of Industry 4.0. Your expertise and creativity will directly influence the evolution of our platform, helping to solve complex technical challenges and drive the future of manufacturing technology.

**Why Contribute?**

**Make an Impact:** Your contributions will help shape a powerful tool that supports businesses worldwide in achieving greater autonomy and efficiency.
Collaborate and Innovate: Work alongside a passionate community of developers and industry experts, sharing knowledge and driving innovation.

**Revenue Sharing:** As a token of our appreciation, contributors will receive a share of the revenue generated from the commercial edition of the iPlus-MES, recognizing your valuable input and dedication.
Join us on GitHub and be part of a project that values collaboration, innovation, and shared success. Together, we can build a more sustainable and independent future for industrial software.

Contribute today and let's create something amazing together!
More about our [rewarding model](https://iplus-framework.com/en/documentation/Read/Index/View/a4100937-4d88-487d-ab3b-e599412e2a2f?workspaceSchemaID=ab0bc53f-decf-4101-9cee-111b6cbc5b24).

## Entity Framework Core

iPlus-MES inherits its data access from the iPlus-framework, which is built on Entity Framework
Core. Long-lived, network-transparent contexts need capabilities that upstream EF Core does not
provide, so **we maintain and publish our own EF Core packages** that contain the fixes:

| Package | Purpose |
|---|---|
| `iPlus.EntityFrameworkCore` | Core, with the fixes below |
| `iPlus.EntityFrameworkCore.Relational` | Relational layer |
| `iPlus.EntityFrameworkCore.SqlServer` | SQL Server provider |
| `iPlus.EntityFrameworkCore.Proxies` | Lazy-loading proxies |
| `iPlus.EntityFrameworkCore.Abstractions` | Abstractions |
| `iPlus.EntityFrameworkCore.Design` / `.Tools` | Design-time tooling |

The fixes these packages carry:

1. **Refresh a context from the database** — [dotnet/efcore#16491](https://github.com/dotnet/efcore/issues/16491)  
   Essential for long-term contexts: already-loaded entities and their relationships can be
   reloaded from the database. For example, when a workflow changed on the client has to be
   reloaded on the server.
2. **Fast `DetectChanges` in long-term contexts** — [dotnet/efcore#31819](https://github.com/dotnet/efcore/issues/31819)  
   Without this, long-lived contexts with large amounts of data become progressively slower.
3. **Reverting changes** — [dotnet/efcore#14594](https://github.com/dotnet/efcore/issues/14594)

### Using them

The packages are consumed from nuget.org and are **enabled by default** — a fresh clone builds
against them with no extra setup. The switch lives in the iPlus-framework repository, in
[build/EntityFramework.Build.props](https://github.com/iplus-framework/iPlus/blob/main/build/EntityFramework.Build.props):

```xml
<UseEFCoreForkIPlus>True</UseEFCoreForkIPlus>
```

Set it to `False` only if you explicitly want the stock `Microsoft.EntityFrameworkCore.*` packages.

> **Note:** you no longer need to clone the EF Core fork. The packages are published on nuget.org,
> so `dotnet restore` is all that is required.

## Documentation

For now, please use the [documentation on our website](https://iplus-framework.com/en/documentation/Home/Schema/View/e0703b3b-647a-46a6-8c53-8706e9cb128b).

We are working on transferring the content to the [iPlus.docs on GitHub](https://github.com/iplus-framework/iPlus.docs) and ask for your patience.

## License

iPlus-MES is **dual-licensed**:

| Edition | License | Intended for |
|---|---|---|
| **Community** | [GPL-3.0](LICENSE) | Development, evaluation and non-commercial use |
| **Commercial** | Commercial license | Productive, commercial operation |

The source code is the same in both editions — the commercial edition adds a runtime license,
support and the right to use the software commercially. See the
[license conditions](https://iplus-framework.com/en/documentation/Read/Index/View/8dec2941-f7ed-4bed-92a5-0e07404e359a?workspaceSchemaID=ab0bc53f-decf-4101-9cee-111b6cbc5b24)
and the [dual licensing overview](https://iplus-framework.com/en?section=Dual%20Licensing#807d323a-35ba-4896-b330-b1cf3d012a0b).

### Building on iPlus-MES

As an integrator you can roll iPlus-MES out to your own customers and extend it for your
industries. The framework ships an integrated license manager so you can issue and enforce licenses
for your own add-ons and products, while the runtime itself is licensed per concurrent user.

- **GPL packages and commercial runtime do not mix.** A package published under GPL-3.0 cannot be
  loaded into a commercially licensed runtime, and vice versa.
- **No developer dongle required.** Development and license issuing are software-based; you sign
  your product licenses with your own key pair.

## Contact

For questions, suggestions, or feedback, please send us a e-mail to **info[at]iplus-framework.com** or [via contact form](https://iplus-framework.com/en?section=Contact#aedf447c-2a3a-4bfe-9e0a-a9c5740e4f8e).

We look forward to your contributions and hope you enjoy working with the iPlus-MES!
