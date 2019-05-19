import React, { Component } from 'react';
import { BrowserRouter, Switch, Route } from 'react-router-dom';
import styled from 'styled-components';

import './App.css';
import { Header } from 'components/Header';
import HomePage from 'containers/homepage';
import Game from 'containers/game';
import GameCreation from 'containers/gameCreation';
import GameInfo from 'containers/gameinfo';
import Profile from 'containers/profile';
import Statistics from 'containers/statistics';
import Tournaments from 'containers/tournament/Loadable';
import SingleTournament from 'containers/tournament/singleTournament';

import { PlayerProvider } from './playersContext';
import { TournamentsProvider } from './tournamentsContext';
import { UserProvider } from './userContext';

const AppWrapper = styled.div`
  margin: 0 auto;
  display: flex;
  min-height: 100%;
  flex-direction: column;
`;

class App extends Component {
  render() {
    return (
      <AppWrapper>
        <BrowserRouter>
          <UserProvider>
            <PlayerProvider>
              <TournamentsProvider>
                <Header>
                  <Switch>
                    <Route exact path="/" component={HomePage}/>
                    <Route path="/token/:token" component={HomePage}/>
                    <Route path="/game/:gameId" component={Game}/>
                    <Route path="/gameInfo/:gameId" component={GameInfo} />
                    <Route path="/game" component={GameCreation} />
                    <Route path="/tournaments/:tournamentId" component={SingleTournament} />
                    <Route path="/tournaments" component={Tournaments} />
                    <Route path="/profile" component={Profile} />
                    <Route path="/statistics" component={Statistics} />
                    {/* <Route path="/goals" component={Goals} /> */}
                    {/* <Route path="/players" component={CrouselContainer} /> 
                    <Route path="" component={NotFoundPage} /> */}
                  </Switch>
                </Header>
              </TournamentsProvider>
            </PlayerProvider>
          </UserProvider>
        </BrowserRouter>
      </AppWrapper>
    );
  }
}

export default App;
