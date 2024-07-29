# NextJS

[https://nextjs.org/](https://nextjs.org/)
[NextJS 14 blog Thursday, October 26th 2023](https://nextjs.org/blog/next-14)

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

## Details

### Add Flexbox Froggy

Flexbox styles. Checkout the app where you can test your styles:

[Flexbox Froggy](https://flexboxfroggy.com/)

React Icons. Install npm package:

```powershell
npm i react-icons

# install tailwindcss/aspect-ratio when using fill property in next/image <Image /> component
npm i -D @tailwindcss/aspect-ratio
```

### Add Countdown Timer to Auction Card

[Getting Started w/react-countdown](https://www.npmjs.com/package/react-countdown)

```powershell
npm i react-countdown
```

### Add Pagination to Auction List

[React Pagination with Flowbite](https://flowbite-react.com/docs/components/pagination)

```powershell
npm i flowbite-react
```

### Zustand state management

[Zustand Documentation](https://docs.pmnd.rs/zustand/getting-started/introduction)

A small, fast, and scalable bearbones state management solution. Zustand has a comfy API based on hooks. It isn't boilerplatey or opinionated, but has enough convention to be explicit and flux-like.

```powershell
npm i zustand
# Package that will Parse a query string into an object.
npm i query-string
```

### Authentication with NextAuth.js

[Documentation - NextAuth (next-auth@4.x.y)](https://next-auth.js.org/)

[Documentation - Auth.js (next-auth@5.0.0-beta)](https://authjs.dev/getting-started)

#### Overview of NextAuth and Auth.js

NextAuth.js is an open-source authentication library for Next.js applications. It provides a flexible and secure way to handle authentication with a wide range of OAuth providers, as well as support for passwordless sign-in. It can be used with or without a database and supports popular databases like MySQL, MongoDB, PostgreSQL, and MariaDB1.

Auth.js is essentially the evolution of NextAuth.js. It was born out of NextAuth but has been developed to be framework-agnostic, meaning it can be used with various frameworks, not just Next.js2. Auth.js builds on the core principles of NextAuth.js but aims to provide a more universal authentication solution.

#### Comparison: NextAuth vs. Auth.js

Framework Dependency:

- NextAuth.js: Specifically designed for Next.js applications.
- Auth.js: Framework-agnostic, can be used with various frameworks.

Configuration:

- NextAuth.js: Configuration is typically done in a file within the pages/api/auth directory.
- Auth.js: Configuration is more flexible and can be placed in a root-level file, making it easier to use across different parts of the application3.

OAuth Support:

- NextAuth.js: Supports a wide range of OAuth providers.
- Auth.js: Continues to support OAuth providers but with stricter compliance to OAuth/OIDC specifications3.

Database Support:

- Both support popular databases and can be used with or without a database.

Migration:

- NextAuth.js: Users of NextAuth.js can migrate to Auth.js with some changes to configuration and imports3.

#### Details of NextAuth v4

[Getting Started](https://next-auth.js.org/getting-started/example)

```powershell
npm install next-auth
```

[Follow directions in this Guide to setup Authentication](https://next-auth.js.org/configuration/initialization#route-handlers-app)

### React Dropdown w/Flowbite React

[React Dropdown - Flowbite (Documentation)](https://flowbite-react.com/docs/components/dropdown)

### Securing NextJS pages and API routes

[Documentation](https://next-auth.js.org/tutorials/securing-pages-and-api-routes)

[Next.js Middleware](https://next-auth.js.org/tutorials/securing-pages-and-api-routes#nextjs-middleware)

With NextAuth.js 4.2.0 and Next.js 12, you can now protect your pages via the middleware pattern more easily.

### Testing NextJS pages and API routes

Create app/session/AuthTest.tsx

[Documentation on how to use and test `getToken`](https://next-auth.js.org/tutorials/securing-pages-and-api-routes#using-gettoken)

### Auction Form using React hook form and react date picker

[React Hook Form](https://react-hook-form.com/)

Add AuctionForm component for creating auctions

The AuctionForm component to the create page in the web app. The AuctionForm component includes form fields for entering the details of a car auction, such as the make and model. It also includes validation logic using the react-hook-form library. This enhancement improves the functionality and user experience of the create page by providing a user-friendly form for creating auctions.

```powershell
# React hook form and react date picker
npm i react-hook-form react-datepicker
# Typescript Types for react date picker
npm i -D @types/react-datepicker
```

### React Hot Toast

```powershell
npm i react-hot-toast
```

### Adding Bids / Notifications
