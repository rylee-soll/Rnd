﻿import {redirect, Route, Routes} from "react-router-dom";
import Root from "./pages/root/Root";
import Login from "./pages/account/Login";
import Register from "./pages/account/Register";
import GamesPage from "./pages/games/GamesPage";
import CharactersPage from "./pages/characters/CharactersPage";
import GamePage from "./pages/games/GamePage";
import Member from "./pages/members/Member";
import CharacterPage from "./pages/characters/CharacterPage";
import {Box, CssBaseline, ThemeProvider} from "@mui/material";
import {Theme} from "./theme";
import AccountRoot from "./pages/account/AccountRoot";
import Profile from "./pages/account/Profile";

export default function App () {
  return (
    <ThemeProvider theme={Theme}>
      <CssBaseline/>
      <Box className="app">
        {/*TODO landing on / path*/}
        <Routes>
          <Route path="account" element={<AccountRoot/>}>
            <Route index element={<Profile/>}/>
            <Route path="login" element={<Login/>}/>
            <Route path="register" element={<Register/>}/>
          </Route>
          <Route path="/" element={<Root/>}>
            <Route index action={() => redirect("/app")}/>
            <Route path="app">
              <Route path="games">
                <Route index element={<GamesPage/>}/>
                <Route path=":gameName" element={<GamePage/>}/>
              </Route>
              <Route path="members">
                <Route path=":username" element={<Member/>}/>
              </Route>
              <Route path="characters">
                <Route index element={<CharactersPage/>}/>
                <Route path=":characterName" element={<CharacterPage/>}/>
              </Route>
            </Route>
          </Route>
        </Routes>
      </Box>
    </ThemeProvider>
  );
}