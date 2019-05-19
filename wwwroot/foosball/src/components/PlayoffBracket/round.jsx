import React from 'react';
import Pair from 'components/PlayoffBracket/pair';

const Round = (props) => (
    <ul className={"round round-" + props.round}>
        {props.round.map((matchup, index) => (
           <Pair matchup={matchup}/>
        ))}
        <li className="spacer">&nbsp;</li>
    </ul>
);

export default Round;