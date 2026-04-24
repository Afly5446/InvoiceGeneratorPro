# Invoice Generator Pro

Desktop **WPF** app for creating invoices: supplier and client fields, line items, **VAT**, live **preview**, **print**, and **PDF export**. UI strings are in Russian; code and this README are in English for portfolio use.

| | |
|---|---|
| **Stack** | .NET 8, WPF, MVVM, [PdfSharp](https://docs.pdfsharp.net/) 6.x, [Handlebars.Net](https://github.com/Handlebars-Net/Handlebars.Net) |
| **Platform** | Windows (`net8.0-windows`) |

## Screenshots

Synthetic previews ship with the repo so the README renders on GitHub immediately. Replace them with real captures (same filenames) using [docs/screenshots/README.md](docs/screenshots/README.md).

![Main window](docs/screenshots/main-window.png)

![Form and validation](docs/screenshots/form-validation.png)

![PDF export](docs/screenshots/pdf-export.png)

## Requirements

- **Windows** 10/11 (x64 recommended).
- To **build from source**: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (includes desktop workload for WPF).
- To **run a published build** from [Releases](https://github.com/YOUR_USER/InvoiceGeneratorPro/releases): no separate runtime if you use the **self-contained** ZIP (see below).

## Build and run (from source)

```bash
cd InvoiceGeneratorPro
dotnet build
dotnet run
```

Or open `InvoiceGeneratorPro.sln` in Visual Studio / Rider and start the **InvoiceGeneratorPro** project.

**First build:** MSBuild runs [Resources/BuildIcon.ps1](Resources/BuildIcon.ps1) (Windows PowerShell 5.1) to generate `Resources/AppIcon.ico`.

## Publish (self-contained, for recruiters)

From the repository root:

```powershell
.\publish.ps1
```

This runs `dotnet publish` for `win-x64` with `--self-contained true` and writes output under `publish/win-x64/`. A ZIP is created under `dist/` for attaching to a GitHub Release.

Manual equivalent:

```bash
dotnet publish InvoiceGeneratorPro.csproj -c Release -r win-x64 --self-contained true -o publish/win-x64
```

Distribute the **folder** or the **ZIP** from `dist/`. The app is **not** code-signed; Windows SmartScreen may show a warning for an unknown publisher—see [RELEASING.md](RELEASING.md).

## Repository and releases

See [RELEASING.md](RELEASING.md) for: creating the GitHub repo, first push, tagging, and uploading the ZIP to **Releases**.

**Resume one-liner (example):**  
*Demo: [Latest Release](https://github.com/YOUR_USER/InvoiceGeneratorPro/releases/latest) — download ZIP, extract, run `InvoiceGeneratorPro.exe`; source: [repository](https://github.com/YOUR_USER/InvoiceGeneratorPro).*

Replace `YOUR_USER` with your GitHub username after publishing. Ready-made CV lines: [docs/RESUME-SNIPPETS.md](docs/RESUME-SNIPPETS.md).

## Third-party licenses

This project uses **PdfSharp** and **Handlebars.Net**; see their respective licenses on NuGet. Project license: [LICENSE](LICENSE) (MIT).

## Synthetic screenshots tooling

```powershell
.\Resources\GenerateReadmeScreenshots.ps1
```

Regenerates PNGs under `docs/screenshots/`.

---

## Кратко (RU)

Десктопное приложение для счетов: НДС, предпросмотр, печать, PDF. Для работодателя: выложите репозиторий на GitHub, прикрепите ZIP из `.\publish.ps1` к **Release**, в резюме дайте ссылку на релиз и на код. Подробно — [RELEASING.md](RELEASING.md).
