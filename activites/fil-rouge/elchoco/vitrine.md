# El Choco · La vitrine

Avant de former le cartel, chaque famille présente sa maison de chocolat et on se regarde les unes les autres.

|           |                                                                                  |
|-----------|----------------------------------------------------------------------------------|
| Prérequis | [D01](../../drills/D01-serialisation.md) · [C1](../../chat/C1-hello.md)          |
| Fallacy   | 3 (bande passante), 8 (réseau homogène)                                          |
| Contrat   | [chocolaterie](chocolaterie.schema.json), topic `elchoco/vitrine/<famille>` (retain) |
| Exigence  | [Sérialisation](../../../thematiques/01-serialisation.md#exigences-du-fil-rouge) |

## Un message valide

Dans l'[enveloppe](../../contrats/envelope.schema.json) du contrat de base, le `type` est `chocolaterie` :

```json
{"v":1,"id":"3d6b1c52-7e0a-4c1e-9f55-2a4c8e6b9d10","from":"maison-dubois","to":"*",
 "ts":"2026-11-04T09:00:00Z","type":"chocolaterie",
 "payload":{"name":"Maison Dubois","zone":["1003","1004","1800"],"capital":250000,
            "capitalCity":"Lausanne","cacao":["criollo","trinitario"],
            "bestProduct":"Truffe au Gruyère","founded":1921,"fairtrade":true}}
```

Le schéma [`chocolaterie.schema.json`](chocolaterie.schema.json) décrit le **payload** seulement (l'enveloppe est déjà validée par le contrat de base). Les six champs de la vitrine sont obligatoires ; `founded`, `fairtrade` et `logo` sont facultatifs.

| Champ | Type | Remarque |
|---|---|---|
| `name` | texte | nom de la maison |
| `zone` | liste de codes postaux | **chaînes** de 4 chiffres : `"1003"`, pas `1003` |
| `capital` | entier | en CHF, sans décimales |
| `capitalCity` | texte | ville du siège (à ne pas confondre avec `capital`) |
| `cacao` | liste, énumération | `criollo`, `trinitario`, `forastero`, `nacional` |
| `bestProduct` | texte | produit phare |

## Mission

1. Définissez votre maison : nom, zone, capital, ville du siège, au moins un cacao, produit phare.
2. **Affichez** votre vitrine à partir de valeurs écrites en dur. Interface libre : terminal ou page web.
3. **Sérialisez** votre maison vers un fichier JSON conforme au schéma, rechargez-la depuis le fichier et supprimez les valeurs en dur.
4. **Publiez** votre vitrine (message `chocolaterie`) sur `elchoco/vitrine/<famille>`.
5. **Abonnez-vous** à `elchoco/vitrine/+` et affichez dans un *Billboard* toutes les maisons reçues.

## DoD

- Je montre ma vitrine et celles d'au moins **deux autres maisons**, dont **une écrite dans un autre langage que le mien**.
- Une maison lancée *après* les autres voit tout de suite les vitrines déjà publiées (sans que personne ne republie).
- Trois entrées hostiles sont ignorées sans effacer le Billboard ni planter le programme :

| Entrée | Exemple | Pourquoi elle est rejetée |
|---|---|---|
| texte non JSON | `bonjour le cartel` | ne se désérialise pas |
| champ manquant | `"payload":{"name":"Maison Dubois","zone":["1003"],"capital":250000,"capitalCity":"Lausanne","cacao":["criollo"]}` | `bestProduct` est obligatoire |
| type inconnu | `"payload":{"name":"Maison Dubois","zone":["1003"],"capital":"beaucoup","capitalCity":"Lausanne","cacao":["criollo"],"bestProduct":"Truffe"}` | `capital` doit être un entier |

## À décider en groupe (et à écrire)

- **Le format** : JSON lisible ou format binaire ? Mesurez la taille d'une vitrine avec et sans `logo` (fallacy 3).
- **Le capital** : pourquoi un entier de francs et pas un `float` ? Que se passe-t-il avec `0.1 + 0.2` dans votre langage ? Et pour les centimes ?
- **Les codes postaux** : pourquoi des chaînes ? Que devient `"1003"` si un langage le lit comme un nombre ? (fallacy 8)

## Indices

- Pourquoi `retain` ici ? (voir [D03](../../drills/D03-qos-retain.md)). Que se passe-t-il quand une maison ferme ? Vous le réglerez avec l'annuaire, plus tard.
- Une image en base64 dans un JSON : c'est le plus simple, est-ce le plus sobre ? Notez un avantage et un inconvénient.
- Une zone qui chevauche celle d'une autre famille est-elle une erreur de **schéma** ou une règle **métier** ? Le schéma peut-il la détecter seul ?

## Pannes à provoquer (labo)

- Ajoutez 2 s de latence (voir [infra](../../infra/README.md)) : votre Billboard reste-t-il réactif ?
- Publiez une vitrine hostile à la main (`cacao: ["sucre"]`, `zone: ["12"]`, JSON cassé) : que se passe-t-il ?
