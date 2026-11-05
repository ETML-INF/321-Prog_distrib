// Shared ETML VitePress helpers. Copy to <project>/.vitepress/etml-vitepress.ts (see SKILL.md).
// Usage in config.mts:
//   import { walk, copyStatic, devPlugins } from './etml-vitepress'
//   const opts = { siteBase: pkg.siteBase, staticDirs, isStatic }
//   vite: { plugins: devPlugins(opts) }
//   buildEnd: (c) => copyStatic(c.outDir, opts)
import { readdirSync, readFileSync, existsSync, mkdirSync, copyFileSync, statSync } from 'node:fs'
import { createServer as createNetServer } from 'node:net'
import path from 'node:path'
import type { Plugin } from 'vite'

export interface EtmlOptions {
  siteBase: string // "/<repo>/"
  staticDirs: string[] // dirs copied as-is (supports, legal, exercices...)
  isStatic: (f: string) => boolean // which files in those dirs are copied/served
}

export function walk(dir: string): string[] {
  return readdirSync(dir).flatMap(f => {
    const p = `${dir}/${f}`
    return statSync(p).isDirectory() ? walk(p) : [p]
  })
}

// buildEnd: copy non-markdown files (docx, zip, pdf, X-*.md...) as-is into the site
export function copyStatic(outDir: string, { staticDirs, isStatic }: EtmlOptions) {
  for (const dir of staticDirs.filter(existsSync)) {
    for (const f of walk(dir).filter(isStatic)) {
      const dest = path.join(outDir, f)
      mkdirSync(path.dirname(dest), { recursive: true })
      copyFileSync(f, dest)
    }
  }
}

// Dev only: serve the static files that buildEnd only copies at build time
function staticAssetsDev({ siteBase, staticDirs, isStatic }: EtmlOptions): Plugin {
  return {
    name: 'serve-static-assets-dev',
    configureServer(server) {
      server.middlewares.use((req, res, next) => {
        const url = decodeURIComponent((req.url ?? '').split('?')[0])
        if (!url.startsWith(siteBase)) return next()
        const rel = url.slice(siteBase.length)
        if (!staticDirs.some(d => rel.startsWith(d + '/')) || rel.includes('..')) return next()
        const f = path.resolve(rel)
        if (!existsSync(f) || !statSync(f).isFile() || !isStatic(rel)) return next()
        if (f.endsWith('.md')) res.setHeader('Content-Type', 'text/plain; charset=utf-8')
        else res.setHeader('Content-Disposition', 'inline')
        res.end(readFileSync(f))
      })
    },
  }
}

// Dev only: host each Slidev deck (slides/<name>.md) in the VitePress process via the @slidev/cli API.
// One Slidev Vite server per deck, created on first access, mounted under <siteBase>slides/<name>/.
function slidevDev({ siteBase }: EtmlOptions): Plugin {
  return {
    name: 'serve-slidev-dev',
    configureServer(server) {
      if (!existsSync('slides')) return
      const servers = new Map<string, Promise<any>>()
      const freePort = () => new Promise<number>((resolve, reject) => {
        const s = createNetServer().once('error', reject).listen(0, () => {
          const { port } = s.address() as { port: number }
          s.close(() => resolve(port))
        })
      })
      const start = async (name: string) => {
        const { createServer, resolveOptions } = await import('@slidev/cli')
        const base = `${siteBase}slides/${name}/`
        const options = await resolveOptions({ entry: `slides/${name}.md`, base }, 'dev')
        return createServer(options, {
          base,
          logLevel: 'warn',
          server: { middlewareMode: true, hmr: { port: await freePort() } },
        })
      }
      const re = new RegExp(`^${siteBase}slides/([^/?]+)/`)
      server.middlewares.use(async (req, res, next) => {
        const m = (req.url ?? '').match(re)
        if (!m || !existsSync(`slides/${m[1]}.md`)) return next()
        try {
          if (!servers.has(m[1])) servers.set(m[1], start(m[1]))
          ;(await servers.get(m[1]))!.middlewares(req, res, next)
        } catch (e) {
          servers.delete(m[1])
          next(e)
        }
      })
      server.httpServer?.on('close', () => servers.forEach(p => p.then(s => s.close())))
    },
  }
}

export const devPlugins = (o: EtmlOptions): Plugin[] => [slidevDev(o), staticAssetsDev(o)]
