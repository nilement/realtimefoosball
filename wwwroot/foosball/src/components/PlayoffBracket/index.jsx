import React from 'react';
import Round from 'components/PlayoffBracket/round';

export default class PlayoffBracket extends React.Component{
    render(){
        if (!this.props.rounds){
            return "Loading brackets";
        }
        return(
            <>
                <main id="tournament">
                    {this.props.rounds.map((round, index) => (
                        <Round round={round}/>
                    ))}
                </main>
            </>
        )
    }
}