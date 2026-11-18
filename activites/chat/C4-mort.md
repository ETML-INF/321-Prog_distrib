# C4 — Annoncer sa propre mort (LWT)

| | |
|---|---|
| Thématique | [Microservices](../../thematiques/04-microservices.md) |
| Drill préalable | [D04](../drills/D04-mort-noeud.md) |
| Fallacy | 1 (fiabilité), 5 (topologie) |
| Théorie | [MQTT](../../supports/mqtt.md#last-will-and-testament-lwt) · [Keep-Alive](../../supports/mqtt.md#keep-alive) |

## But
Quand votre chat meurt, les autres le savent sans que vous ayez rien fait.

## Étapes

**1. Déclarez un testament à la connexion.** Topic `chat/presence/<votre prénom>`, QoS 1, **retain**, contenu : une enveloppe `presence` avec `"state":"died"`. Keep-alive : **5 s**.  
→ Attention : le testament est construit **au moment de la connexion**. Son `id` et son `ts` seront ceux de cet instant. Est-ce un problème ? Notez votre réponse.

**2. Mort franche.** Tuez votre chat (`kill -9`) comme dans D04.  
→ Vérifiez : un observateur (`DC mosquitto_sub -t 'chat/presence/#' -v`) voit `died` en 1 à 2 s.

**3. Mort silencieuse.** Connectez votre chat **via le proxy** (`localhost:1884`), puis `docker compose exec toxiproxy /toxiproxy-cli toxic add -t timeout -a timeout=0 mqtt`. Chronométrez.  
→ Vérifiez : `died` apparaît après ≈ 8 à 10 s. Retirez ensuite le toxic.

**4. Affichez-le.** Votre liste des présents (C3) passe l'auteur en `mort` à la réception de `died`.  
→ Vérifiez avec 2 instances : tuez l'une, l'autre affiche « X est mort ».

**5. Renaissance.** Relancez le chat tué.  
→ Vérifiez : à sa connexion il publie `open` (retain), qui **écrase** `died` ; les autres le voient revenir.

**6. Message empoisonné.** À la main (`mosquitto_pub`), publiez sur `chat/presence/<votre prénom>` un `presence` **invalide**, au choix : `"state":"dead"`, `id` absent, ou `"id":"nope"`.  
→ Vérifiez : l'autre chat **ignore** le message (la liste des présents ne change pas), le **logge** et incrémente `bad`. Un chat qui plante ou qui affiche l'état invalide n'a pas terminé. Théorie : [Valider à la frontière](../../supports/microservice.md#valider-a-la-frontiere).

<details><summary>Indices : valider en C# et en JS</summary>

Un schéma par `type`, **chargé au démarrage**. On valide le message entier contre `envelope`, puis son `payload` contre le schéma de son `type`. Attention : sans l'assertion de format, `"id":"nope"` passe.

**C#** (`dotnet add package JsonSchema.Net`) :

```csharp
using System.Text.Json;
using Json.Schema;

var opts = new EvaluationOptions { RequireFormatValidation = true }; // format assertif
var envelope = JsonSchema.FromText(File.ReadAllText("envelope.schema.json"));
var schemas = new Dictionary<string, JsonSchema> {
    ["presence"] = JsonSchema.FromText(File.ReadAllText("presence.schema.json")),
};

bool Valid(string json) {
    using var doc = JsonDocument.Parse(json);
    var msg = doc.RootElement;
    if (!envelope.Evaluate(msg, opts).IsValid) return false;
    var type = msg.GetProperty("type").GetString()!;
    return schemas.TryGetValue(type, out var s)
        && s.Evaluate(msg.GetProperty("payload"), opts).IsValid;
}
```

**JS** (`npm i ajv ajv-formats`) :

```js
import Ajv2020 from "ajv/dist/2020.js";
import addFormats from "ajv-formats";
import fs from "fs";

const ajv = new Ajv2020({ strict: false });
addFormats(ajv); // format assertif
const load = f => JSON.parse(fs.readFileSync(f));
const envelope = ajv.compile(load("envelope.schema.json"));
const schemas = { presence: ajv.compile(load("presence.schema.json")) };

function valid(msg) {
  if (!envelope(msg)) return false;
  const s = schemas[msg.type];
  return !!s && s(msg.payload);
}
```

Dans les deux cas, un `type` sans schéma est ignoré et loggé (règle 4 du contrat). Testé avec JsonSchema.Net 9.7.0 et Ajv 8.20 / ajv-formats 3.0.1.
</details>

## Terminé quand

- [ ] les deux morts sont détectées par un autre chat
- [ ] vous pouvez expliquer pourquoi le délai diffère
- [ ] la renaissance (étape 5) fonctionne
- [ ] un message invalide est ignoré, loggé et compté dans `bad`

## Si vous utilisez une IA
Prompt : nommer LWT, keep-alive, retain, et donner **les deux scénarios de test** (franche, silencieuse) avec le délai attendu pour chacun, ainsi que la validation du message contre son schéma (format assertif).
