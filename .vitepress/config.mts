import { defineConfig } from 'vitepress'
import { withMermaid } from 'vitepress-plugin-mermaid'
import { glob } from 'glob'
import path from 'path'
import pkg from '../package.json'

// Fail build when siteBase doesn't match the actual GitHub repo name (CI only)
if (process.env.GITHUB_REPOSITORY) {
  const repoName = process.env.GITHUB_REPOSITORY.split('/')[1]
  const expected = `/${repoName}/`
  if (pkg.siteBase !== expected) {
    throw new Error(`[config] siteBase mismatch: package.json="${pkg.siteBase}" vs GITHUB_REPOSITORY implies "${expected}"`)
  }
}

process.env.VITE_EXTRA_EXTENSIONS = 'docx,pdf,pptx,zip,csv,xlsx'

const repoUrl = process.env.GITHUB_REPOSITORY
  ? `https://github.com/${process.env.GITHUB_REPOSITORY}`
  : 'https://github.com/ETML-INF/321-Prog_distrib'

const toItems = (pattern: string) =>
  glob.sync(pattern, { posix: true, ignore: ['**/README.md', 'node_modules/**'] })
    .sort()
    .map(f => ({ text: path.basename(f, '.md'), link: '/' + f.replace(/\.md$/, '') }))

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

  markdown: { math: true },

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
  ignoreDeadLinks: false,
}))
