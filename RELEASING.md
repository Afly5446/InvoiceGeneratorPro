# Publishing Invoice Generator Pro for your resume

This guide completes the **GitHub repository** and **Release** steps. Replace `YOUR_USER` / `YOUR_REPO` with your values.

## 1. Create a public GitHub repository

1. On GitHub: **New repository** → name e.g. `InvoiceGeneratorPro` → **Public** → create without README (this repo already has one).
2. On your PC (from the project folder that contains `InvoiceGeneratorPro.sln`):

```bash
git init
git add .
git commit -m "Initial commit: Invoice Generator Pro WPF portfolio app"
git branch -M main
git remote add origin https://github.com/YOUR_USER/InvoiceGeneratorPro.git
git push -u origin main
```

If the project folder is nested inside another git repo, either use a dedicated clone folder for this app only, or add this folder as the root of a new repo (avoid nested `.git` unless you know you need it).

## 2. Build the distributable ZIP

```powershell
.\publish.ps1
```

Outputs:

- `publish/win-x64/` — folder with `InvoiceGeneratorPro.exe` and dependencies.
- `dist/InvoiceGeneratorPro-win-x64.zip` — archive to upload.

Test locally: extract the ZIP to a temp folder and run `InvoiceGeneratorPro.exe`. Ideally test on another PC or VM without the .NET SDK installed (self-contained build).

## 3. Create a GitHub Release

### Option A: GitHub website

1. **Releases** → **Draft a new release**.
2. **Choose a tag**: create e.g. `v1.0.0` on `main`.
3. **Release title**: `v1.0.0` (or `Initial portfolio release`).
4. **Describe this release** (example):

   ```markdown
   Self-contained Windows x64 build. Extract the ZIP and run `InvoiceGeneratorPro.exe`.
   No .NET runtime install required.
   ```

5. Attach **`dist/InvoiceGeneratorPro-win-x64.zip`**.
6. Publish release.

### Option B: GitHub CLI (`gh`)

```bash
gh auth login
gh release create v1.0.0 dist/InvoiceGeneratorPro-win-x64.zip --title "v1.0.0" --notes "Self-contained Windows x64. Extract ZIP, run InvoiceGeneratorPro.exe."
```

## 4. SmartScreen (unsigned app)

The published `.exe` is **not** Authenticode-signed. Windows may show **Windows protected your PC**. For reviewers, add to README/Release notes:

> The application is not code-signed. If SmartScreen appears, use **More info** → **Run anyway**.

Commercial code signing is optional and paid.

## 5. Resume / cover letter

Copy-paste templates (English / Russian) live in **[docs/RESUME-SNIPPETS.md](docs/RESUME-SNIPPETS.md)** — replace `YOUR_USER` / `YOUR_REPO` with your GitHub paths.

Quick reference:

- **Source:** `https://github.com/YOUR_USER/InvoiceGeneratorPro`
- **Try the app:** `https://github.com/YOUR_USER/InvoiceGeneratorPro/releases/latest`

## 6. Optional polish

- Replace synthetic PNGs in `docs/screenshots/` with real screenshots.
- Add a 1–2 minute screen recording (YouTube / Loom) and link it from the README.
