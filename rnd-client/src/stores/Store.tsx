﻿import Session from "./Session";
import Module from "./models/Module";
import {client} from "../data/Client";
import Collection from "./Collection";

export class Store {
  constructor() {
    this.session = new Session();
    this.modules = new Collection<Module>(client.modules(this.user), Module)
    this.modules.load();
  }

  get user(): string {
    return this.session.user?.name ?? "@guest";
  }

  session: Session
  modules: Collection<Module>
}

export const store = new Store();