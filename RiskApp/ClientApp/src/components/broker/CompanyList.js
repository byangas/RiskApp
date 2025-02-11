import React, {useEffect, useRef, useState} from "react";
import CompanyService from "../../services/CompanyService";
import {Button} from "react-bootstrap";
import debounce from "../utils/debounce";
import CompanyTile from "./CompanyTile";

//this has to be outside of the exported "function" because otherwise it get recreated every render cycle
const searchByCompanyNameDebounced = debounce((searchCriteria,filterByAppointed, callback) => {
    CompanyService.getCompanyList(searchCriteria, filterByAppointed).then(companies => {
        callback(companies)
    })
}, 1500);


export default function CompanyList() {

    const [companyList, setCompanyList] = useState([])
    const [filterByAppointedCarriers, setFilterByAppointedCarriers] = useState(true)
    const txtSearch = useRef();
    useEffect(() => {
        CompanyService.getCompanyList(null, filterByAppointedCarriers).then(companies => {
            setCompanyList(companies)
        })
    }, [])

    useEffect(()=> {
        searchByCompanyName();
    },[filterByAppointedCarriers])

    //for when the search criteria changes
    function searchByCompanyName() {
        searchByCompanyNameDebounced(txtSearch.current.value, filterByAppointedCarriers, setCompanyList)
    }
    const companies = companyList.map(company => {
        return <CompanyTile company={company}/>
    })
    return (
        <>
            <h1>Company List</h1>
            Search By Company Name &nbsp;
            <input type="text" ref={txtSearch} onChange={(e) => searchByCompanyName()} placeholder="Search"/><Button
            className="btn-inline" onClick={()=> {
            txtSearch.current.value = ""
            searchByCompanyName()
        } }>Clear Search</Button>
            <div>
                <span>
                    <label htmlFor="rdoAppointed">Appointed Carriers</label>
                    <input type="radio" name="filterBy" value="appointed"
                           onChange={()=> setFilterByAppointedCarriers(true)}
                           defaultChecked={filterByAppointedCarriers}
                           id="rdoAppointed"/>&nbsp;
                </span>
                <span>
                    <label htmlFor="rdoAll">All Carriers</label>
                    <input  type="radio" name="filterBy"
                            value="all"
                            onChange={()=> setFilterByAppointedCarriers(false)}
                            defaultChecked={!filterByAppointedCarriers}
                            id="rdoAll"/>

                </span>
            </div>
            <div className="tile-container">

                {companies}
            </div>

        </>

    )
}
