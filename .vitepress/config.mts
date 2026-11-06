import { defineConfig } from 'vitepress'
import { withMermaid } from 'vitepress-plugin-mermaid'
import { glob } from 'glob'
import path from 'path'
import type MarkdownIt from 'markdown-it'
import pkg from '../package.json'
import { copyStatic, devPlugins } from './etml-vitepress'

// Fail build when siteBase doesn't match the actual GitHub repo name (CI only)
if (process.env.GITHUB_REPOSITORY) {
  const repoName = process.env.GITHUB_REPOSITORY.split('/')[1]
  const expected = `/${repoName}/`
  if (pkg.siteBase !== expected) {
    throw new Error(`[config] siteBase mismatch: package.json="${pkg.siteBase}" vs GITHUB_REPOSITORY implies "${expected}"`)
  }
}

process.env.VITE_EXTRA_EXTENSIONS ??= 'docx,pdf,pptx,zip,csv,xlsx,json,yaml,conf'

// Fichiers non-markdown copiés tels quels (build) et servis tels quels (dev)
const staticDirs = ['activites', 'umddv2', 'supports']
const isStatic = (f: string) => !f.endsWith('~') && !f.endsWith('.md') && !/\/(bin|obj)\//.test(f)
const etml = { siteBase: pkg.siteBase, staticDirs, isStatic }

const repoUrl = process.env.GITHUB_REPOSITORY
  ? `https://github.com/${process.env.GITHUB_REPOSITORY}`
  : 'https://github.com/ETML-INF/321-Prog_distrib'

const toItems = (pattern: string) =>
  glob.sync(pattern, { posix: true, ignore: ['**/README.md', 'node_modules/**'] })
    .sort()
    .map(f => ({ text: path.basename(f, '.md'), link: '/' + f.replace(/\.md$/, '') }))

// README.md est réécrit en index.md (cf. rewrites) : les liens vers README.md/README.html
// (écrits pour GitHub) doivent pointer vers le dossier, sinon ils donnent /README.html (404)
function readmeLinksPlugin(md: MarkdownIt) {
  md.core.ruler.push('readme-links', (state) => {
    for (const blockToken of state.tokens) {
      if (blockToken.type !== 'inline' || !blockToken.children) continue
      for (const token of blockToken.children) {
        if (token.type !== 'link_open') continue
        const href = token.attrGet('href') ?? ''
        if (/^[a-z][a-z0-9+.-]*:/i.test(href)) continue // liens externes
        const fixed = href.replace(/(^|\/)README\.(?:md|html)(?=[#?]|$)/, '$1')
        if (fixed !== href) token.attrSet('href', fixed || './')
      }
    }
  })
}

// https://vitepress.dev/reference/site-config
export default withMermaid(defineConfig({
  lang: 'fr-CH',
  title: 'ICT-321',
  description: 'Module ICT 321 - Programmer des systèmes distribués',
  base: pkg.siteBase,

  // Slidev sources are built separately into dist/slides
  srcExclude: ['slides/**', 'node_modules/**', 'docs/superpowers/**'],

  rewrites: {
    'README.md': 'index.md',
    ':dir/README.md': ':dir/index.md',
    ':dir/:sub/README.md': ':dir/:sub/index.md',
    ':dir/:sub/:sub2/README.md': ':dir/:sub/:sub2/index.md',
  },

  vite: { plugins: devPlugins(etml) },

  // Fichiers non-markdown (json, zip, pdf...) copiés tels quels dans le build
  buildEnd: (c) => copyStatic(c.outDir, etml),

  markdown: { math: true, config: (md) => { readmeLinksPlugin(md) } },

  themeConfig: {
    nav: [
      { text: 'Accueil', link: '/' },
      { text: 'Thématiques', link: '/thematiques/' },
      { text: 'Supports', link: '/supports/' },
      { text: 'Activités', link: '/activites/chat/' },
    ],
    sidebar: {
      '/': [
        { text: 'Thématiques', collapsed: false, items: toItems('thematiques/*.md') },
        { text: 'Supports', collapsed: true, items: toItems('supports/*.md') },
        {
          text: 'Fil rouge',
          collapsed: true,
          items: [
            { text: 'Choisir son scénario', link: '/activites/fil-rouge/' },
            { text: 'Catalogue de services', link: '/activites/fil-rouge/services' },
            { text: 'Fil rouge libre', link: '/activites/fil-rouge/libre/' },
            { text: 'Gabarit de scénario', link: '/activites/fil-rouge/modele/' },
            { text: 'Évaluation', link: '/evaluation/grille' },
          ],
        },
        {
          text: 'Activités',
          collapsed: true,
          items: glob.sync('activites/*/README.md', { posix: true })
            .sort()
            .map(f => ({ text: f.split('/')[1], link: '/' + f.replace(/README\.md$/, '') })),
        },
      ],
    },
    socialLinks: [{ icon: 'github', link: repoUrl }],
    search: { provider: 'local' },
    outline: { label: 'Sur cette page' },
    docFooter: { prev: 'Précédent', next: 'Suivant' },
    returnToTopLabel: 'Retourner au début',
  },

  // Les liens du contenu sont écrits pour la navigation GitHub (ex: ../../umdd, hors site) : on ne bloque pas le build
  // Les slides Slidev sont construits à part (dist/slides) : leurs liens ne sont pas des pages VitePress
  ignoreDeadLinks: [/slides\//, /\.(pdf|xlsx|docx|csv|pptx|zip|json|yaml|conf)$/i],
}))
