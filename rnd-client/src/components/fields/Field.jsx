﻿import TextField from "./TextField";

export default function Field({type, ...props}) {
  switch (type) {
    case "text": return(<TextField {...props}/>)
    default: return(<TextField {...props}/>)
  }
}