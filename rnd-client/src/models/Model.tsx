﻿
export default class Model {
  constructor(data: {name: string, path: string, [key:string]: any}) {
    this.name = data.name;
    this.path = data.path;
    this.title = data.title ?? null;
    this.description = data.description ?? null;
    this.color = data.color ?? null;
    this.icon = data.icon ?? null;
    this.image = data.image ?? null;
    this.backgroundImage = data.background ?? null;

    this.created = data.created ?? new Date();
    this.edited = data.edited ?? new Date();
    this.viewed = data.viewed ?? new Date();
    this.deleted = data.deleted ?? null;
  }

  // BaseModel
  name: string
  path: string
  title: string | null
  description: string | null
  color: string | null
  icon: string | null
  image: string | null
  backgroundImage: string | null

  // History
  readonly created: Date
  edited: Date
  viewed: Date
  deleted: Date | null
}