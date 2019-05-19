import React from 'react';

const Pair = ({ matchup }) => (
    <>
        <li className="spacer">&nbsp;</li>
        <li className="game game-top">{matchup.t1P1 ? matchup.t1P1.player.name : "TBD"}<span>{matchup.t1P1 ? matchup.wins1 : "TBD"}</span></li>
        <li className="game game-spacer">&nbsp;</li>
        <li className="game game-bottom ">{matchup.t2P1 ? matchup.t2P1.player.name : "TBD"}<span>{matchup.t2P1 ? matchup.wins2 : "TBD"}</span></li>
    </>
);

export default Pair;