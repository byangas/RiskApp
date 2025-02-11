import React, {useContext} from 'react';
import {BrowserRouter, Route, Switch} from 'react-router-dom';
import ContactTile from "./components/broker/contact/ContactTile";
import Profile from './components/Profile';
import Navigation from './components/Navigation'

import Spinner from './components/controls/Spinner'
import {shimValueAsNumber} from "./ShimValueAsNumber";
import UserContext from "./UserContext";
import AddBrokerContact from "./components/broker/contact/AddBrokerContact";
import Customers from "./components/broker/Customers";
import CustomerDetail from "./components/broker/CustomerDetail";
import UserMessages from "./components/UserMessages";
import ContactManagement from "./components/broker/ContactManagement";
import Company from "./components/Company";
import PolicyEdit from "./components/broker/PolicyEdit";
import PolicyNew from "./components/broker/PolicyNew";
import MarketingSheetAddContact from "./components/broker/MarketingSheetAddContact";
import AppetiteFitFormEdit from "./components/broker/AppetiteFitFormEdit";
import Supplementals from "./components/Supplementals";
import ContactView from "./components/broker/contact/ContactView";
import Dashboard from "./components/broker/Dashboard";

shimValueAsNumber();

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');

export default function App() {

    let DefaultHome = Dashboard;

    const {user} = useContext(UserContext)

    if(user) {
        // for the moment, a cheap hack to get the carrier user experience out there
        // for now, we only have "messages" as a feature for the carrier experience, so we default to
        // message view
        if(!user.broker) {
            DefaultHome = UserMessages;
        }
    }

    if (!user) {
        return (<Spinner/>)
    }

    return (
        <BrowserRouter basename={baseUrl}>
            <Navigation userRoles={user}/>
            <Switch>
                <Route exact path='/' component={DefaultHome}/>
                <Route exact path='/broker/contacts' component={ContactManagement}/>
                <Route exact path='/broker/contacts/add' component={AddBrokerContact}/>
                <Route exact path='/broker/contacts/:contactId' component={ContactView}/>
                <Route exact path='/broker/customer' component={Customers}/>
                <Route exact path='/broker/customer/new' component={CustomerDetail} />
                <Route exact path='/broker/customer/:customerId' component={CustomerDetail} />
                <Route exact path='/broker/customer/:customerId/policy/new' component={PolicyNew} />
                <Route exact path='/broker/customer/:customerId/policy/:policyId/' component={PolicyEdit} />
                <Route exact path='/broker/customer/:customerId/policy/:policyId/marketing' component={MarketingSheetAddContact} />
                <Route exact path='/broker/customer/:customerId/policy/:policyId/appetite' component={AppetiteFitFormEdit} />

                <Route exact path='/broker/customer/:customerId/policy/:policyId/:state' component={PolicyEdit}  />
                <Route exact path='/broker/contacts/company/:companyId' component={Company} />

                <Route exact path='/user/messages/' component={UserMessages} />
                <Route exact path='/profile' component={Profile}/>
                <Route exact path='/Account/SignOut' component={DefaultHome}/>
                <Route exact path='/user/forms/' component={Supplementals}/>
            </Switch>
        </BrowserRouter>
    );

}