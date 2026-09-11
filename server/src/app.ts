import express, { Request, Response } from "express";
import cors from "cors";
import { DinoStore, ValidationError } from "./store.js";

export function createApp(store: DinoStore = new DinoStore()) {
  const app = express();
  app.use(cors());
  app.use(express.json());

  app.get("/api/health", (_req: Request, res: Response) => {
    res.json({ status: "ok", world: "mundo-dino" });
  });

  app.get("/api/dinos", (_req: Request, res: Response) => {
    res.json({ dinos: store.list() });
  });

  app.get("/api/dinos/:id", (req: Request, res: Response) => {
    const dino = store.get(req.params.id);
    if (!dino) {
      res.status(404).json({ error: "Dinossauro nao encontrado." });
      return;
    }
    res.json({ dino });
  });

  app.post("/api/dinos", (req: Request, res: Response) => {
    try {
      const dino = store.add(req.body);
      res.status(201).json({ dino });
    } catch (err) {
      if (err instanceof ValidationError) {
        res.status(400).json({ errors: err.errors });
        return;
      }
      res.status(500).json({ error: "Erro interno ao criar dinossauro." });
    }
  });

  return app;
}
