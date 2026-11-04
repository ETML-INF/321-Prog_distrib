# D01 — Sérialisation : ça pèse combien, qu'est-ce qui casse ?

|          |                                                                                                     |
|----------|-----------------------------------------------------------------------------------------------------|
| Thématique | [Sérialisation](../../thematiques/01-serialisation.md) |
| Fallacy  | 3 (bande passante infinie), 8 (réseau homogène)                                                     |
| Durée    | 20 min                                                                                              |
| Matériel | un éditeur de texte, un validateur JSON Schema (en ligne ou outil de votre langage)                 |
| Théorie  | [Sérialisation](../../supports/Sérialisation.md) (surtout *Binaire VS Texte* et *Défis et limites*) |

## But
Mesurer le coût réel d'un message, et constater qu'un changement de nom de champ casse un autre groupe.

## Étapes

1. **Écrivez à la main** (sans outil de génération) le JSON de ce commerce : nom `Chez Léa`, une spécialité `Croissant` à `1.8`. Respectez le [schéma catalogue](catalogue.schema.json). Enregistrez-le en UTF-8 dans `business.json`, **sans retour à la ligne final**, sans espaces inutiles.  
   → Fichier de la forme `{"name":"Chez Léa","specialties":[{"name":"Croissant","price":1.8}]}`.

2. **Comptez les octets** (`wc -c business.json`, ou propriétés du fichier).  
   → **69 octets**. Pas 68 : `é` pèse 2 octets en UTF-8. Remplacez par `e`, vous obtenez 68.

3. **Réécrivez-le indenté** (2 espaces par niveau, une propriété par ligne) et recomptez.  
   → environ **107 octets** : même information, +55 % de poids. Qui paie cette lisibilité ?

4. **Validez** votre fichier avec le schéma `catalogue.schema.json`.  
   → valide ✔. Mettez `"price":"1.8"` (texte) : le validateur refuse. C'est le contrat qui parle, pas votre code.

5. **Décodez une version binaire** (MessagePack) du même objet, 60 octets :

   ```
   82 a4 6e 61 6d 65 a9 43 68 65 7a 20 4c c3 a9 61 ab 73 70 65 63 69 61 6c 74 69 65 73
   91 82 a4 6e 61 6d 65 a9 43 72 6f 69 73 73 61 6e 74 a5 70 72 69 63 65
   cb 3f fc cc cc cc cc cc cd
   ```

   Tableau de lecture : `82` = table de 2 entrées · `91` = tableau de 1 élément · `a0 + n` = texte de *n* octets · `cb` = nombre décimal sur 8 octets qui suit.  
   Table ASCII : `6e 61 6d 65` = `name`, `43` = `C`.  
   💡 **Astuce** : n'additionnez pas les octets de tête. Collez ces octets dans un lecteur hexadécimal (ex. [CyberChef](https://gchq.github.io/CyberChef/) avec *From Hex*, `xxd -r -p | xxd`, ou un éditeur hexa comme HxD / l'extension *Hex Editor* de VS Code) : la colonne de droite affiche le texte ASCII et vous repérez `name`, `Chez L`, `specialties`… d'un coup d'œil. Les octets non imprimables (`82`, `a4`, `c3`…) apparaissent comme `.` : ce sont eux que le tableau de lecture vous aide à interpréter.  
   Retrouvez la valeur de `name` de la première table, puis l'octet qui dit « 9 octets de texte ».  
   → `a9` annonce 9 octets : `Chez Léa` (`4c c3 a9 61` = `L`, `é` sur 2 octets, `a`).

6. **Comparez** : 69 octets (JSON compact) contre 60 (binaire).  
   → seulement −13 %. Les noms de champs (`name`, `specialties`, `price`) sont **toujours dans le message**.

7. **Ajoutez l'enseigne** : une image de 3000 octets, transmise en base64 dans le JSON.  
   → base64 = 4 octets pour 3, soit **4000 octets** + guillemets. L'image pèse maintenant 98 % du message.

8. **Évolution** : un autre groupe renomme `price` en `cost`. Reprenez votre `business.json` d'origine et validez-le contre *leur* schéma (copiez le vôtre, renommez la propriété `price` en `cost`, y compris dans `required`).  
   → refusé : propriété `cost` obligatoire absente.

## Débrief (à l'oral, 5 min)

- Pour quel type de réseau le binaire vaut-il le coup ? Pour lequel ne vaut-il pas le coup ? (fallacies 3 et 7)
- Un groupe écrit en Python, l'autre en Rust. Pourquoi le JSON aide-t-il ? (fallacy 8)
- Qu'est-ce qui aurait évité l'étape 8 ? Pensez à `v` dans l'[enveloppe](../contrats/README.md).

<details><summary>Solution</summary>

- Octets : 69, ≈107, 60, ≈4100 (69 + ≈4000 + clés).
- Le binaire sans schéma ne s'auto-décrit pas : sans le tableau de lecture vous ne lisez rien. Des formats comme Protobuf remplacent les noms de champs par des numéros, d'où un gain bien supérieur, au prix d'un schéma partagé à distribuer.
- La version du contrat (`v`) permet d'accepter l'ancien et le nouveau format le temps d'une migration.
</details>
