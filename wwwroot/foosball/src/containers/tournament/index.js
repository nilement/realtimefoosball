import React from 'react';
import { PlayersContext } from 'containers/App/playersContext';
import { TournamentsContext } from 'containers/App/tournamentsContext';
import TournamentCreation from './tournamentCreation';
import TournamentsInfo from './tournamentsInfo';

class Tournaments extends React.Component{
    constructor(){
        super();
        this.setCreating = this.setCreating.bind(this);
    }

    state = {
        creating: false,
        tournamentPlayers: [],
        tournamentName: '',
        numberOfGroups: false
    }

    setCreating(){
        this.setState({ creating: true });
    }

    render(){
        return (
            <PlayersContext.Consumer>
                {({ loading, error, players }) => {
                    if (loading) return <div>fuck</div>;
                    if (error) return <div>error</div>;
                    return (
                        <TournamentsContext.Consumer>
                        {({ loading: tournamentsLoading, error: tournamentsError, tournaments }) => {
                            if (tournamentsLoading) return <div>fuck</div>;
                            if (tournamentsError) return <div>error</div>;
                            return(
                                <>
                                    {!this.state.creating &&
                                        <TournamentsInfo tournaments={tournaments} setCreating={this.setCreating} />
                                    }
                                    {this.state.creating && 
                                        <TournamentCreation players={players}/>
                                    }
                                </>
                            )
                        }}
                        </TournamentsContext.Consumer>
                    )
                }}
            </PlayersContext.Consumer>
        )
    }
}

export default Tournaments;