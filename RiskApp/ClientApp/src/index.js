import 'bootstrap/dist/css/bootstrap.css';
import './index.css';
import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import UserProvider from "./UserProvider";

const rootElement = document.getElementById('root');

ReactDOM.render(<UserProvider><App/></UserProvider>, rootElement);

