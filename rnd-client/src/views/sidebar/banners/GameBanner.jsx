﻿import {useStore} from "../../../stores/StoreProvider";
import {observer} from "mobx-react-lite";
import {usePath} from "../../../hooks";
import ShowGame from "./gameStates/ShowGame";
import NotActiveGame from "./gameStates/NotActiveGame";
import NoneGame from "./gameStates/NoneGame";
import ActiveGame from "./gameStates/ActiveGame";

const State = {
  Show: "Show",
  Active: "Active",
  NotActive: "NotActive",
  None: "None",
}

const GameBanner = observer(() => {
  const game = useStore().session.game;

  let state = State.Show;
  if (!game) state = State.None;
  else if (usePath(game.path)) state = State.Active;
  else if (usePath("modules/:name")) state = State.NotActive;

  switch (state) {
    case State.Show:
      return (<ShowGame game={game}/>)
    case State.Active:
      return (<ActiveGame/>)
    case State.NotActive:
      return (<NotActiveGame/>)
    default:
      return (<NoneGame/>)
  }
});

export default GameBanner