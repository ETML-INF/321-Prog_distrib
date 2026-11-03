// Échoue si un lien Markdown `fichier.md#ancre` pointe vers une ancre inexistante.
// VitePress retire les accents des identifiants de titres : écrire `#contrat-de-scenario`, pas `#contrat-de-scénario`.
// Usage : node scripts/check-anchors.mjs
import fs from 'fs'
import path from 'path'

const root = process.cwd()
const skip = new Set(['node_modules', '.git', 'dist', 'cache', 'slides', 'docs', '.vitepress'])
const files = []
const walk = d => {
  for (const e of fs.readdirSync(d, { withFileTypes: true })) {
    if (skip.has(e.name)) continue
    const p = path.join(d, e.name)
    if (e.isDirectory()) walk(p)
    else if (p.endsWith('.md')) files.push(p)
  }
}
walk(root)

// Même règle que VitePress (markdown-it-anchor)
const slug = s => s.trim()
  .replace(/<[^>]+>/g, '').replace(/[`*]/g, '')
  .normalize('NFKD').replace(/[̀-ͯ]/g, '')
  .replace(/[\s~`!@#$%^&*()\-_+=\[\]{}|\;:"'“”‘’<>,.?\/]+/g, '-')
  .replace(/-{2,}/g, '-').replace(/^-+|-+$/g, '').toLowerCase()

const anchors = {}
for (const f of files) {
  const t = fs.readFileSync(f, 'utf8')
  const a = new Set()
  for (const m of t.matchAll(/^#{1,6}\s+(.*?)(?:\s*\{#([^}]+)\})?\s*$/gm)) {
    a.add(slug(m[1]))
    if (m[2]) a.add(m[2])
  }
  for (const m of t.matchAll(/id="([^"]+)"/g)) a.add(m[1])
  anchors[f] = a
}

let bad = 0
for (const f of files) {
  const t = fs.readFileSync(f, 'utf8')
  for (const m of t.matchAll(/\]\(([^)\s]+?\.md)#([^)\s]+)\)/g)) {
    const target = path.resolve(path.dirname(f), decodeURIComponent(m[1]))
    const anchor = decodeURIComponent(m[2])
    if (anchors[target] && !anchors[target].has(anchor)) {
      console.log(`${path.relative(root, f)} -> ${m[1]}#${anchor}`)
      bad++
    }
  }
}
if (bad) { console.log(`KO: ${bad} ancre(s) cassée(s)`); process.exit(1) }
console.log('OK')
