# NextJS

[https://nextjs.org/](https://nextjs.org/)

## Overview

The training course developed the frontend application with NextJS v13.
However, this repository has taken the pragmatic view of developing the frontend application with NextJS v14.

Overview of features:

- Performance
- Load times with lazy loading and pre-fetching
- Good SEO due to Server side rendering of html
- Acts as BFF for our client app
- Store Secret on Server used to fetch requests from backend APIs for BFF functionality
- React based
- Web Browser Application

## Create NextJS project

```powershell

npx create-next-app@latest
√ What is your project named? ... web-app
√ Would you like to use TypeScript? ... No / Yes
√ Would you like to use ESLint? ... No / Yes
√ Would you like to use Tailwind CSS? ... No / Yes
√ Would you like to use `src/` directory? ... No / Yes
√ Would you like to use App Router? (recommended) ... No / Yes
√ Would you like to customize the default import alias (@/*)? ... No / Yes
Creating a new Next.js app in C:\DEV\github.com\CarAuctionNextNet\frontend\web-app.

Using npm.

Initializing project with template: app-tw


Installing dependencies:
- react
- react-dom
- next

Installing devDependencies:
- typescript
- @types/node
- @types/react
- @types/react-dom
- postcss
- tailwindcss
- eslint
- eslint-config-next

npm WARN deprecated inflight@1.0.6: This module is not supported, and leaks memory. Do not use it. Check out lru-cache if you want a good and tested way to coalesce async requests by a key value, which is much more comprehensive and powerful.
npm WARN deprecated @humanwhocodes/config-array@0.11.14: Use @eslint/config-array instead
npm WARN deprecated rimraf@3.0.2: Rimraf versions prior to v4 are no longer supported
npm WARN deprecated glob@7.2.3: Glob versions prior to v9 are no longer supported
npm WARN deprecated @humanwhocodes/object-schema@2.0.3: Use @eslint/object-schema instead

added 361 packages, and audited 362 packages in 19s

136 packages are looking for funding
  run `npm fund` for details

found 0 vulnerabilities
Success! Created web-app at C:\DEV\github.com\CarAuctionNextNet\frontend\web-app

```

## Next.js Version 13 vs. Version 14

- [Next.js 13 Blog](https://nextjs.org/blog/next-13)
- [Next.js 13 — Features, Changes and Improvements](https://medium.com/naukri-engineering/next-js-13-features-changes-and-improvements-1483831a1909)

Key Differences:

- Server Actions and Partial Prerendering: Next.js 14 introduces Server Actions and Partial Prerendering, enhancing performance and developer experience1.
- Minimum Node.js Version: Version 14 requires Node.js 18.17 (since 16.x has reached end-of-life) compared to 16.14 in version 132.
- Export Command Change: The next export command is removed in favor of the output: 'export' config2.
- ImageResponse Import Rename: The next/server import for ImageResponse is renamed to next/og2.
- Font Package Removal: The @next/font package is fully removed in favor of the built-in next/font2.
- WASM Target Removal: The WASM target for next-swc is removed2.
