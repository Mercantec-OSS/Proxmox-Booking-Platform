## Description
This is the documentation for the [VMware-frontend](https://gitlab.pcvdata.dk/team/vmware-frontend) project. 

The purpose of the frontend is to serve a web application ([booking.merhot.dk](https://booking.merhot.dk/)) that allows users to log in with their Mercantec Active Directory credentials and manage cluster and virtual machines.

## Tech stack
- [Svelte](https://svelte.dev/) - Framework for building fast and reactive web applications.
- [SvelteKit](https://kit.svelte.dev/) - Full-stack framework for Svelte with routing and SSR.
- [Tailwind CSS](https://tailwindcss.com/) - CSS framework for easier and better CSS styling.
- [shadcn-svelte](https://www.shadcn-svelte.com/) - UI components library for ported to Svelte, styled with Tailwind CSS.
- [Lucide icons](https://lucide.dev/) - Open-source, customizable, icon library.

## File structure
```
├── components.json       // shadcn-svelte component config
├── index.js              // Node.js server to start production environment
├── jsconfig.json         // JavaScript config to manage paths etc.
├── package.json          // List of npm packages and project config
├── package-lock.json     // Npm package details
├── postcss.config.js     // Tailwind CSS plugins
├── README.md             // Project description
├── src                   // Main SvelteKit project folder
│   ├── app.css           // General theme stylings such as custom color classes
│   ├── app.html          // HTML template with header etc.
│   ├── hooks.server.js   // Hook to get user's auth token from cookie
│   ├── lib               // Contains components, controllers, middlewares, utils
│   │   ├── components    // Svelte components such as navbar etc.
│   │   │   ├── authed    // Components for pages requiring auth
│   │   │   ├── ui        // shadcn-svelte UI components
│   │   │   └── unauthed  // Components for pages that don't require auth
│   │   ├── controllers   // Functions to fetch data
│   │   ├── middlewares   // Middlewares to check if user is authenticated
│   │   └── utils         // Miscellaneous functions
│   └── routes            // Routing, paths are based on folder names
│       ├── (authed)      // Routes for authenticated users
│       ├── (public)      // Routes accessible by both authed and unauthed users
│       └── (unauthed)    // Routes for unauthenticated users
├── static                // Static assets, optimized in production build
├── svelte.config.js      // SvelteKit config
├── tailwind.config.js    // Tailwind CSS config
└── vite.config.js        // Vite config with proxy settings for development
```

## Setup for development
To clone the project in order to get the files onto your computer:

> **Note:** If you don't have Node.js installed, you can download and install it from [here](https://nodejs.org/en/download/).

```bash
git clone https://gitlab.pcvdata.dk/team/vmware-frontend
cd vmware-frontend
npm install
```

Copy the `.env.example` file and rename it to `.env`, then modify the variables to your needs.

(copy command on Linux)
```bash
cp .env.example .env
```

Create a proxy to back end API by modifying `vite.config.js`:
```javascript
import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';
export default defineConfig({
  plugins: [sveltekit()],
  server: {
    proxy: {
      '/api': {
        target: 'http://10.132.128.20:6002/',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api/, '')
      }
    },
    host: '0.0.0.0'
  }
});
```

```bash
npm run dev
```

## Setup for production
To clone the project in order to get the files onto your computer:

> **Note:** If you don't have Node.js installed, you can download and install it from [here](https://nodejs.org/en/download/).

```bash
git clone https://gitlab.pcvdata.dk/team/vmware-frontend
cd vmware-frontend
npm install
```

Copy the `.env.example` file and rename it to `.env`, then modify the variables to your needs.

(copy command on Linux)
```bash
cp .env.example .env
```

To create a compiled version of the project:
```bash
npm run build
```

Run the compiled version of the project
```bash
node index.js
```

## Miscellaneous
- [Backend repository](https://gitlab.pcvdata.dk/team/vmware-backend): ASP.NET backend providing API endpoints for the frontend.
- [Automation repository](https://gitlab.pcvdata.dk/team/vmware-automation): ASP.NET project handling communication between physical hardware and backend.
- [Production repository](https://gitlab.pcvdata.dk/team/vmware-docker-compose): Combines frontend, backend, and automation into a single Docker instance.
- [Trello board](https://trello.com/invite/b/uiXM5dtg/ATTId46fe21fd956d78843f87807af9c4f368DBDB4D7/vmware-hosting): Used to assign and manage tasks among team members

> Authors:
- [Jesper Nielsen](mailto:jeni.skp@edu.mercantec.dk?subject=vmware-frontend)
- [Viktor Viskov](mailto:vikt3586@edu.mercantec.dk?subject=vmware-frontend)
- [Rasmus Frøstrup](mailto:rkfr.skp@edu.mercantec.dk?subject=vmware-frontend)
