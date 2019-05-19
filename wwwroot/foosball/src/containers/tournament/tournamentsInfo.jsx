import React from 'react';
import { Link } from 'react-router-dom';
import { UserContext } from 'containers/App/userContext';

export default class TournamentsInfo extends React.Component{
    render(){
        let { tournaments, setCreating } = this.props;
        return(
            <UserContext.Consumer>
                {({ authed, token }) => (
                    <div>
                        <div>Running Tournaments</div>
                        {tournaments.map((tournament, index) => (
                            <div key={index}>
                                <span>{tournament.name} </span>
                                <span>{tournament.startDate} </span>
                                <span>{tournament.groupCount}</span>
                                {authed && <Link to={"/tournaments/"+tournament.id}>Tournament info</Link>}
                            </div>
                        ))}
                        <button onClick={setCreating}>Create new tournament</button>
                    </div>
                )}
            </UserContext.Consumer>
        )
    }
}