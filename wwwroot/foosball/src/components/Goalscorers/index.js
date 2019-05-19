/**
 *
 * Goalscorers
 *
 */

import React from 'react';
import { BarChart, XAxis, YAxis, Bar, Tooltip, Legend } from 'recharts';

// import PropTypes from 'prop-types';
// import styled from 'styled-components';

function Goalscorers(props) {
  var data = props.data.map(player => ({ 'name': player.player.name, 'Goals': player.goals }));
  return (
    <div>
        <h2>Top goalscorers</h2>
        <BarChart width={800} height={300} data={data}>
            <XAxis dataKey="name"/>
            <YAxis />
            <Bar dataKey="Goals" fill="#8884d8" />
            <Tooltip />
            <Legend />
        </BarChart>
    </div>
  );
}

Goalscorers.propTypes = {};

export default Goalscorers;
