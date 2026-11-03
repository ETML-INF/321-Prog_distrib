#!/usr/bin/env bash
# Échoue si un nom de scénario apparaît dans le cœur du module.
cd "$(dirname "$0")/.." || exit 2
PAT='dousville|umdd|powercher|pedalo|\bBP\b|boulang'
paths=(activites/drills activites/chat activites/infra activites/contrats supports)
out=$(grep -rnIiE "$PAT" "${paths[@]}" --include='*.md' --include='*.json' --include='*.yaml' --include='*.conf' 2>/dev/null | grep -v '~:')
out2=$(grep -nIiE "$PAT" evaluation/*.md 2>/dev/null | grep -v "annexes/")
all="$out$out2"
if [ -n "$all" ]; then echo "$all"; echo "KO: noms de scénario dans le cœur"; exit 1; fi
echo OK
