--
-- PostgreSQL database dump
--

-- Dumped from database version 11.12
-- Dumped by pg_dump version 11.12

-- Started on 2022-02-14 12:23:25

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 2 (class 3079 OID 289315)
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- TOC entry 4369 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


--
-- TOC entry 642 (class 1247 OID 289394)
-- Name: action_type; Type: TYPE; Schema: public; Owner: riskappadmin
--

CREATE TYPE public.action_type AS ENUM (
    'create',
    'update',
    'delete',
    'message',
    'related'
);


ALTER TYPE public.action_type OWNER TO riskappadmin;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 201 (class 1259 OID 289326)
-- Name: account; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.account (
    account_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    password character varying(256) NOT NULL,
    create_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public.account OWNER TO riskappadmin;

--
-- TOC entry 206 (class 1259 OID 289377)
-- Name: account_role; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.account_role (
    account_id uuid NOT NULL,
    application_role_id uuid NOT NULL,
    created_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public.account_role OWNER TO riskappadmin;

--
-- TOC entry 207 (class 1259 OID 289405)
-- Name: activity; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.activity (
    activity_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    entity_id uuid NOT NULL,
    activity_type public.action_type NOT NULL,
    created_by uuid NOT NULL,
    created_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    description character varying(2056) NOT NULL,
    company_id uuid
);


ALTER TABLE public.activity OWNER TO riskappadmin;

--
-- TOC entry 202 (class 1259 OID 289333)
-- Name: application_role; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.application_role (
    application_role_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    name character varying(256) NOT NULL
);


ALTER TABLE public.application_role OWNER TO riskappadmin;

--
-- TOC entry 209 (class 1259 OID 289424)
-- Name: broker_account; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.broker_account (
    broker_account_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    title character varying(200) NOT NULL,
    primary_contact_name character varying(200),
    industry character varying(200) NOT NULL,
    industry_details character varying(1000) NOT NULL,
    address character varying(200) NOT NULL,
    city character varying(200) NOT NULL,
    state character varying(50) NOT NULL,
    zip character varying(25) NOT NULL,
    email character varying(200),
    phone character varying(200),
    created_by_profile_id uuid NOT NULL,
    created_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    information text
);


ALTER TABLE public.broker_account OWNER TO riskappadmin;

--
-- TOC entry 203 (class 1259 OID 289339)
-- Name: company; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.company (
    company_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    company_name character varying(1024) NOT NULL,
    company_type character varying(1024) NOT NULL,
    email_domain character varying(2048) NOT NULL,
    company_website character varying(1024) NOT NULL,
    company_logo character varying(1024) NOT NULL
);


ALTER TABLE public.company OWNER TO riskappadmin;

--
-- TOC entry 211 (class 1259 OID 289449)
-- Name: marketing_sheet_contact; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.marketing_sheet_contact (
    marketing_sheet_contact_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    policy_id uuid NOT NULL,
    profile_id uuid NOT NULL,
    status character varying(50) DEFAULT 'new'::character varying NOT NULL,
    created_by_profile_id uuid NOT NULL,
    premium integer,
    commission integer,
    created_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public.marketing_sheet_contact OWNER TO riskappadmin;

--
-- TOC entry 212 (class 1259 OID 289457)
-- Name: marketing_sheet_contact_note; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.marketing_sheet_contact_note (
    marketing_sheet_contact_note_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    marketing_sheet_contact_id uuid NOT NULL,
    note text NOT NULL,
    created_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public.marketing_sheet_contact_note OWNER TO riskappadmin;

--
-- TOC entry 213 (class 1259 OID 289467)
-- Name: message; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.message (
    message_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    sender_profile_id uuid NOT NULL,
    send_to_profile_id uuid NOT NULL,
    subject_id uuid NOT NULL,
    message_type character varying(25) NOT NULL,
    message text NOT NULL,
    created_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    status character varying(25) DEFAULT 'new'::character varying NOT NULL,
    read_date timestamp with time zone
);


ALTER TABLE public.message OWNER TO riskappadmin;

--
-- TOC entry 210 (class 1259 OID 289434)
-- Name: policy; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.policy (
    policy_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    broker_account_id uuid NOT NULL,
    renewal_date time with time zone,
    status character varying(50) NOT NULL,
    insurance_types character varying(50)[] NOT NULL,
    created_by_profile_id uuid NOT NULL,
    created_date time with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    detail jsonb NOT NULL
);


ALTER TABLE public.policy OWNER TO riskappadmin;

--
-- TOC entry 204 (class 1259 OID 289348)
-- Name: profile; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.profile (
    profile_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    account_id uuid,
    company_id uuid,
    first_name character varying(255),
    last_name character varying(255),
    phone character varying,
    email character varying(255) NOT NULL,
    create_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    title character varying(255) DEFAULT ''::character varying NOT NULL,
    specialty character varying(255)
);


ALTER TABLE public.profile OWNER TO riskappadmin;

--
-- TOC entry 208 (class 1259 OID 289417)
-- Name: profile_contact; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.profile_contact (
    profile_contact_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    profile_id uuid NOT NULL,
    company_id uuid NOT NULL,
    invalid_date timestamp with time zone,
    created_by uuid NOT NULL,
    created_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public.profile_contact OWNER TO riskappadmin;

--
-- TOC entry 205 (class 1259 OID 289361)
-- Name: registration; Type: TABLE; Schema: public; Owner: riskappadmin
--

CREATE TABLE public.registration (
    registration_id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    created_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    registration_code character varying(255) NOT NULL,
    expires_date timestamp with time zone NOT NULL,
    account_id uuid,
    company_id uuid,
    accept_date timestamp with time zone,
    email character varying(255) NOT NULL,
    roles uuid[] NOT NULL
);


ALTER TABLE public.registration OWNER TO riskappadmin;

--
-- TOC entry 4351 (class 0 OID 289326)
-- Dependencies: 201
-- Data for Name: account; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.account (account_id, password, create_date) FROM stdin;
170281f5-92d0-41ae-b408-8398429b3fbf	$2a$11$l6e76T53EGIQOgKjf4/hk.5NfF.Xr5/C50e7tKiFytoML7Qo9DHc.	2022-01-09 06:34:12.349126+00
152e756e-3cf0-42a3-ae70-56300544fbb9	$2a$11$2UwXi8JspA5OplrrdaIYc.Nuhr78Mb23A3W..GOfVmPI4wchc0iWa	2022-01-09 18:27:44.626972+00
c8c980a6-5523-4701-a26e-082080055c17	$2a$11$5WIqClMBTcqp39mNuoGzw.IyWf1YCCew63QlL80sgM2pSslhwQAGa	2022-01-25 23:45:00.910707+00
5e49428f-ada2-4e22-9a59-d9c2e8dba35c	$2a$11$LypdKUhKgnWiEfRScYOKE.dJPq4Tc2mbRI9kYA.KzLKGfd/7aa5/G	2022-01-27 01:03:24.816011+00
\.


--
-- TOC entry 4356 (class 0 OID 289377)
-- Dependencies: 206
-- Data for Name: account_role; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.account_role (account_id, application_role_id, created_date) FROM stdin;
170281f5-92d0-41ae-b408-8398429b3fbf	2b3d5d19-adb5-41d8-b0bf-e225e36d49af	2022-01-09 06:34:12.349126+00
152e756e-3cf0-42a3-ae70-56300544fbb9	1397b033-4553-40e7-b4a8-8f63a325f35a	2022-01-09 18:27:44.626972+00
c8c980a6-5523-4701-a26e-082080055c17	1397b033-4553-40e7-b4a8-8f63a325f35a	2022-01-25 23:45:00.910707+00
5e49428f-ada2-4e22-9a59-d9c2e8dba35c	1397b033-4553-40e7-b4a8-8f63a325f35a	2022-01-27 01:03:24.816011+00
\.


--
-- TOC entry 4357 (class 0 OID 289405)
-- Dependencies: 207
-- Data for Name: activity; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.activity (activity_id, entity_id, activity_type, created_by, created_date, description, company_id) FROM stdin;
\.


--
-- TOC entry 4352 (class 0 OID 289333)
-- Dependencies: 202
-- Data for Name: application_role; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.application_role (application_role_id, name) FROM stdin;
dd797b4d-51fe-4161-ae9f-742ff32742b6	ROLE_SYSTEM_ADMIN
f5a40d96-9418-4b38-99ef-ced5aa45b4af	ROLE_COMPANY_OWNER
1397b033-4553-40e7-b4a8-8f63a325f35a	ROLE_BROKER
2b3d5d19-adb5-41d8-b0bf-e225e36d49af	ROLE_CARRIER_REP
\.


--
-- TOC entry 4359 (class 0 OID 289424)
-- Dependencies: 209
-- Data for Name: broker_account; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.broker_account (broker_account_id, title, primary_contact_name, industry, industry_details, address, city, state, zip, email, phone, created_by_profile_id, created_date, information) FROM stdin;
6c615dde-905d-44a1-8c1e-a786235bb2ea	VIP Pizza	Chris	Accommodation and Food Services	delivery, wine and beer only	3615 Brookside Road	Stockton	CA	95219	chris@vippizza.com	209-477-7854	9cc7e6fd-3378-46e5-a98c-68425581ea80	2022-01-11 21:31:25.874154+00	no claims 5 years
de0b108e-63d3-4563-9a23-a4cfd60b6cef	RR Insurance Brokers	Robert	Finance and Insurance	Insurance Broker	3415 Brookside Rd	Stockton	CA	95219	bryan@dohrins.com	2097127395	b7ec0868-17f7-4e77-8046-20c3a24536f3	2022-01-27 01:05:29.614374+00	Hi
\.


--
-- TOC entry 4353 (class 0 OID 289339)
-- Dependencies: 203
-- Data for Name: company; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.company (company_id, company_name, company_type, email_domain, company_website, company_logo) FROM stdin;
ee9c5b78-251f-4577-8601-8332fa0066e6	RiskMinute	carrier	riskminute.com	https://www.riskminute.com	riskminute.svg
7df043d5-d1da-4593-8339-8ff049932ed4	CNA	carrier	cna.com	https://www.cna.com	cna.png
335305af-a5c7-40c2-b6cc-594165825a92	American Family Insurance	carrier	amfam.com	https://www.amfam.com	american_family.png
882fac1b-768d-4d64-b744-3a6efca6edbc	Nationwide	carrier	nationwide.com	https://www.nationwide.com	nationwide.png
61ef07b9-4e79-48df-9f2d-7f5773cee24c	Dohrmann Insurance	broker	dohrins.com	https://www.dorhins.com	dorhmann.png
0c585f30-df80-4530-86b9-af90d7d8b59d	ICW	carrier	icwgroup.com	https://www.icwgroup.com/	ICWGroupLogobw.png
ab5ec825-7117-4bd1-a908-d81de0505028	Risico	carrier	risico.com	https://risico.com/	risicologobw.png
253d1d11-edd3-4678-bae1-7836b79b7689	Liberty Mutual	carrier	libertymutual.com	https://www.libertymutual.com/	LibertyMutualLogobw.png
8b0f9f5c-bc68-43a2-9f88-d9ce65b33880	Midwest	carrier	midins.com	https://www.midins.com/	midwestlogobw.png
93c97712-730a-4bed-a444-5684fd517b2c	Patriot National	carrier	patriotnational.com	http://patriotnational.com/	PNUlogobw.png
c0449272-f234-40ea-9d52-5bed21213c6f	PIE Insurance	carrier	pieinsurance.com	https://pieinsurance.com/	pieinsurancelogobw.png
261a525e-44b3-4ca7-8952-865804903653	Republic Indemnity	carrier	ri-net.com	https://www.republicindemnity.com/	republicindemitylogobw.png
ddc143bb-79a5-4a00-90cc-d53721cdb32e	The Zenith Insurance	carrier	thezenith.com	https://www.thezenith.com/	Zenithlogobw.png
2960a1b4-6ecf-407a-90a9-d7d93b06e0f6	Travelers	carrier	travelers.com	https://www.travelers.com/	travelerslogobw.png
c742fe40-8e99-4488-a7fa-695759ac4594	Fireman's Fund	carrier	firemansfund.com	https://www.hmia.com/insurance-company/firemans-fund	firemansfundlogobw.png
c01e3bf8-0b18-4348-b194-f4dcfb21141f	JJ Negley	carrier	jjnegley.com	https://www.jjnegley.com/	NegleyLogobw.png
c12af9ba-0d29-40f8-955e-ff77b74924fa	Omaha National	carrier	omahanational.com	https://www.omahanational.com/	omahanationallogobw.png
4a26cc09-4bb2-4bb1-988d-13937fab72ea	Mid Western Insurance	carrier	mwiainsurance.com	https://midwesterninsurance.com/	Midwesternbw.png
313fc125-3006-4857-8285-f2c35e9d7961	Amtrust	carrier	amtrustgroup.com	https://amtrustfinancial.com/	amtrustbw.png
2e733b06-e82a-420a-9656-5c01fa51f1b4	Atlas	carrier	atlas.us.com	https://www.atlas.us.com/	Atlas_Logo_2021_Color_Website.webp
f7030d01-56e1-4611-aa0d-37352a83f2af	Applied Underwriters	carrier	auw.com	https://www.auw.com/	AppliedUnderwritersLogobw.png
0f778ae4-d1bb-4d2a-ae15-40f4cd56c6b9	Hartford	carrier	thehartford.com	https://www.thehartford.com/	Hartfordlogobw.png
488394dd-ac32-4f91-aae0-9ba2e36854b7	Berkshire Hathaway Homestate Insurance	carrier	bhhc.com	https://www.bhhc.com/	berkshirelogobw.png
960ac3ca-c9c6-4533-8467-79c27c335688	All Risks	carrier	allrisks.com	https://www.allrisks.com	RSG-Logo-RGB-03192020.png
04f21f2d-9c82-43db-bb40-bff104123669	Hanover	carrier	hanover.com	https://www.hanover.com/	hanoverlogobw.png
2c7aaa02-f45c-4dfd-a99b-87d41ab5caa1	Capital Insurance Group	carrier	ciginsurance.com	https://www.ciginsurance.com/	cigbw.png
59fc5d42-19f0-4fc3-a693-16ad4fc0610c	Builders Insurance Services	carrier	insuranceBIS.com	https://insurancebis.com/	BIS_logo_color.png
4b4203f2-0176-4feb-8f33-f1a66b45255d	Crum & Forster	carrier	cfins.com	https://www.cfins.com/	cfins_logo_white.svg
e3717d9d-f8df-4eec-a150-85b90d1a2050	AMTrust Financial	carrier	amtrustfinancial.com	https://amtrustfinancial.com/	amtrust-financial_white.svg
a38d5278-b7ae-47c9-a9bb-69d67455d810	NAU Country	carrier	naucountry.com	https://naucountry.com/	naucountry_color.png
548b9b69-8195-4cf5-8f26-61ebb867d0aa	PAK Programs 	carrier	pakprograms.com	https://pakprograms.com/	pakprograms_color.png
092f3085-13f3-45fd-a0cf-e50dceac3af5	Fireman's Fund	carrier	ffic.com	https://www.hmia.com/insurance-company/firemans-fund	firemansfundlogobw.png
05befd4f-d2ee-4e58-9de3-d79ffb7fa9d5	Market Scout	carrier	marketscout.com\t	https://marketscout.com/	MarketScout-logo-white-horizontal.png
2a30cedb-0251-4c00-913e-902b83bde19a	Arrowhead Exchange	carrier	arrowheadgrp.com	https://www.arrowheadgrp.com/	arrowhead_color.png
b57c37b0-0b41-405b-a9bf-6051c1ad4069	Rain and Hail	carrier	rainhail.com	https://www.rainhail.com/	rainhail_transparent_blue_300.png
f9b7667c-5b72-4db8-937c-7a717d0d2982	QBE North America	carrier	us.qbe.com	https://www.qbe.com/us	qbe_white.svg
121d3680-22e5-4c8e-96fc-dbeca02677ae	Breckenridge	carrier	breckis.com	https://www.breckis.com/	breckenridge_olor_Icon.png
0e50e0f8-74f7-4a8b-b552-7b8661d57ca3	Breckenridge (Formerly Blue River)	carrier	blueriveruw.com	https://www.breckis.com/	breckenridge_olor_Icon.png
7ddabfa7-ef67-479e-8a84-811b638d9bc8	BTIS	carrier	btisinc.com	https://my.btisinc.com/	btis_logo-white.svg
3a8c4553-ea5d-43c4-9efb-fc55de0b6fa8	Allianz	carrier	agcs.allianz.com	https://www.allianz.com/	allianzbw.png
def13dee-1715-424c-9876-9b88a0c5a315	Chubb Agribusiness	carrier	chubbagribusiness.com	https://www.chubbagribusiness.com/	chubbbw.png
b69622a2-96ef-4fed-a8bc-b2137452a2ba	Chubb	carrier	chubb.com	https://www.chubb.com/	chubbbw.png
9e92fbcb-d687-47af-b888-ba5a4377da6b	Capital Insurance Group	carrier	ciginsurance.com	https://www.ciginsurance.com/	cigbw.png
c3e8b861-9306-4313-b521-be75a5d5b303	EMC Insurance	carrier	emcins.com	https://www.emcins.com/	emc_color.jpg
5ad61eee-084a-420f-a90a-e8aa11f82713	Acuity Insurance	carrier	acuity.com	https://www.acuity.com/	acuity-logo-300x100.png
411c3ca5-0231-4619-ab73-038adfe4298a	AF Group	carrier	afgroup.com	https://www.afgroup.com/	AFGroup_bw.svg
24ab371a-5d50-4416-8d9e-e2db09a090ad	AIG	carrier	aig.com	https://www.aig.com/	aig_logo_color.png
52a5fdeb-1548-47e3-9360-1b9e55ba3b9d	Alfa Insurance	carrier	alfainsurance.com	https://www.alfainsurance.com/	Alfa_logo_color.svg
4ae2755c-7b9d-4d09-8f3d-a78066f761b1	Ally Insurance	carrier	ally.com	https://allyinsurance.com/	ally_insurance_bw.svg
c11d7f3d-724d-48ca-acef-b24fc6f30f76	AmericanAg	carrier	aaic.com	https://www.aaic.com/	americanag_logo_color.jpg
cb3eb3ca-8e5b-42df-82a3-0ff195d9c0ca	American National Insurance	carrier	americannational.com	https://www.americannational.com/	american_national_color.png
90cd5305-ed7a-4c9f-8ad2-6db85cadac03	American Transit Insurance	carrier	american-transit.com	https://american-transit.com/	american_transit_color.png
3379376d-3801-485e-b7e4-e1f6ef03d90b	American Pet Insurance	carrier	americanpetinsurance.com	http://americanpetinsurance.com/	americanpetinsurance_logo_color.png
858ff772-3bf4-4b39-a89d-587cb308c34d	Amerisafe	carrier	amerisafe.com	https://www.amerisafe.com/	amerisafe.com.svg
7e60a500-c2d4-4337-9d39-ce2f352ad090	Amerisure	carrier	amerisure.com	https://www.amerisure.com/	amerisure.com.webp
584f28cb-5577-425b-927d-36d7a0fad4b3	Ameritrust Group	carrier	ameritrustgroup.com	https://www.ameritrustgroup.com/	ameritrustgroup.com.png
697b0314-fc9f-4bf3-a7d2-141be6b6e44a	State Compensation Insurance Fund	carrier	scif.com	https://www.statefundca.com/	scif_bw.png
ea9b5b70-b986-49d1-8025-22fb68b56058	American Reliable	carrier	americanreliable.com	https://americanreliable.com/	americanreliableag.com.png
87612457-909e-4773-93e2-5cbd70204486	Amica	carrier	amica.com	https://www.amica.com/	amica.com.svg
cfd7fc78-e15b-41dd-bf6b-aa3aa33fe939	Andover Companies	carrier	andovercompanies.com	https://www.andovercompanies.com/	andovercompanies.com.svg
e9557ba9-c86f-4fab-bad2-d169fef04ed9	Arbella Insurance	carrier	arbella.com	https://www.arbella.com/	arbella.com.png
39fdb475-9eaf-4459-b988-2aef9782a073	Arch Insurance	carrier	archinsurance.com	https://insurance.archgroup.com/	archinsurance.com.svg
cbc47198-d9b0-4c9e-8dfb-66c06d0c8676	Argo Group	carrier	argogroupus.com	https://www.argolimited.com/	argogroupus.com.svg
c0e28929-e0dc-4bb7-9c15-ad598d9fec73	Aspen	carrier	aspen-insurance.com	https://www.aspen.co	aspen.co.svg
8f7fa6d2-656c-49ac-adce-bd87ccd43899	Axis Capital	carrier	axiscapital.com	https://www./	axis.com.svg
37d049ba-3eee-4e87-932f-b77d0a1e2e18	Berkshire Hathaway	carrier	berkshirehathaway.com	https://berkshirehathaway.com/	NO LOGO
82cd787b-2892-41fe-84a7-41036fcdc21c	National Indemnity	carrier	nationalindemnity.com	https://www.nationalindemnity.com/	nationalindemnity.com.png
754bb37b-387c-42ca-a1c0-99a32625111b	Brotherhood Mutual	carrier	brotherhoodmutual.com	https://www.brotherhoodmutual.com/	brotherhoodmutual.com.png
672a5532-b62b-42e1-85c2-4e2d74117e15	Beazley	carrier	beazley.com	https://www.beazley.com/	beazley.com.png
a64caabf-2170-43a0-b352-58049b3e6fbd	Builders Mutual	carrier	buildersmutual.com	https://www.buildersmutual.com/	buildersmutual.com.jpg
7738962c-e849-4e0f-9833-9ebfdde70562	California Earthquake Authority	carrier	calquake.com	https://www.earthquakeauthority.com/	earthquakeauthority.com.png
3542dc5b-4671-4dcc-8c6b-5cd39195ae9d	Central Insurance	carrier	central-insurance.com	https://www.central-insurance.com/	central-insurance.com.svg
85e81bef-dde4-4840-abe7-6077c36cc0e9	Cherokee Insurance	carrier	insurecherokee.com	https://insurecherokee.com/	insurecherokee.com.png
73d6a213-66a6-41c7-91e4-3d24494f41d4	Church Mutual Insurance	carrier	churchmutual.com	https://www.churchmutual.com/	churchmutual.com.png
332d5bbc-bbb5-4fb9-97e4-bc61e92ec470	Cincinnati Insurance	carrier	cinfin.com	https://www.cinfin.com/	cinfin.com.png
7faa1a7f-d8ac-4007-ace8-504b7ca8a97e	CopperPoint Ins Group	carrier	copperpoint.com	https://www.copperpoint.com/	copperpoint.com.png
342d67d5-585b-463d-817d-4e2c5c28d093	Country Financial	carrier	countryfinancial.com	https://www.countryfinancial.com/	countryfinancial.com.png
38541e74-1b8b-48e8-a955-b1180a6540b8	Coverys	carrier	coverys.com	https://www.coverys.com/	coverys.com.svg
5c1e7134-0e79-4599-a607-76a9e6f0cecf	Diamond State Group	carrier	diamondstategroup.com	https://www.diamondstategroup.com/	NO LOGO
68cd1bf7-1bc0-4619-a452-475f81341afe	Global Indemnity	carrier	global-indemnity.com	https://gbli.com/	global-indemnity.com.png
310a8ce2-7d00-4956-83b6-51ebf4fe77a2	The Doctors Company	carrier	thedoctors.com	https://www.thedoctors.com/	thedoctors.com.svg
fded6792-431f-4a4b-9a87-8739dc9b2f98	Donegal Insurance Group	carrier	donegalgroup.com	https://donegalgroup.com/	donegalgroup.com.jpg
4b6f8d40-8d29-4d5f-8430-893fa98d23b1	Employers	carrier	employers.com	https://www.employers.com/	employerslogobw.png
069de3f5-e53c-48df-944a-fdfc2690408b	Mutual of Enumclaw	carrier	mutualofenumclaw.com	https://mutualofenumclaw.com/	mutualofenumclaw.com.png
5003398d-cc79-47d4-a54f-5723d10ef40c	Encova	carrier	encova.com	https://www.encova.com/	encova.com.png
3644a551-6b24-476c-90e9-f128474003f7	Erie Insurance	carrier	erieinsurance.com	https://www.erieinsurance.com/	erieinsurance.com.svg
fb730b16-7374-4b07-9919-ba2a18c8539a	Everest	carrier	everestre.com	https://www.everestre.com/	everestbw.png
ec56c2cb-a03c-4e73-a17b-3056b79dc9b9	Idaho Farm Bureau Insurance	carrier	idfbins.com	https://www.idahofarmbureauinsurance.com/	idahofarmbureauinsurance.com.png
0a3e46b3-356f-4cca-af52-79d9a4181f7b	Core Specialty	carrier	corespecialtyinsurance.com	https://corespecialtyinsurance.com/	corespecialtyinsurance.com.svg
\.


--
-- TOC entry 4361 (class 0 OID 289449)
-- Dependencies: 211
-- Data for Name: marketing_sheet_contact; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.marketing_sheet_contact (marketing_sheet_contact_id, policy_id, profile_id, status, created_by_profile_id, premium, commission, created_date) FROM stdin;
99af1f7b-7353-4248-bcb6-0754d18a473d	f9d0cdfb-7d72-4f51-ac8a-1f581c8759c8	55d7f804-2d25-4ba9-9206-9c63e58f9bd8	quote	9cc7e6fd-3378-46e5-a98c-68425581ea80	\N	\N	2022-01-11 21:37:15.518605+00
\.


--
-- TOC entry 4362 (class 0 OID 289457)
-- Dependencies: 212
-- Data for Name: marketing_sheet_contact_note; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.marketing_sheet_contact_note (marketing_sheet_contact_note_id, marketing_sheet_contact_id, note, created_date) FROM stdin;
\.


--
-- TOC entry 4363 (class 0 OID 289467)
-- Dependencies: 213
-- Data for Name: message; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.message (message_id, sender_profile_id, send_to_profile_id, subject_id, message_type, message, created_date, status, read_date) FROM stdin;
9cf910f7-edb8-430c-8484-5f2c9b398761	9cc7e6fd-3378-46e5-a98c-68425581ea80	55d7f804-2d25-4ba9-9206-9c63e58f9bd8	99af1f7b-7353-4248-bcb6-0754d18a473d	AppetiteFitRequest	check this out, new opporunity	2022-01-11 21:38:58.348029+00	new	2022-01-11 21:40:48.364957+00
bbea0385-ede6-4d48-bd95-94260e45b017	55d7f804-2d25-4ba9-9206-9c63e58f9bd8	9cc7e6fd-3378-46e5-a98c-68425581ea80	99af1f7b-7353-4248-bcb6-0754d18a473d	AppetiteFitResponse	Yes, this fits our appetite. Please apply.	2022-01-11 21:40:48.364957+00	new	\N
2941ccaf-b410-4f0d-b009-7ea024416390	9cc7e6fd-3378-46e5-a98c-68425581ea80	55d7f804-2d25-4ba9-9206-9c63e58f9bd8	99af1f7b-7353-4248-bcb6-0754d18a473d	Default	Let's meet at VIP for lunch so you can check it out.	2022-01-11 21:42:57.147804+00	new	\N
\.


--
-- TOC entry 4360 (class 0 OID 289434)
-- Dependencies: 210
-- Data for Name: policy; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.policy (policy_id, broker_account_id, renewal_date, status, insurance_types, created_by_profile_id, created_date, detail) FROM stdin;
f9d0cdfb-7d72-4f51-ac8a-1f581c8759c8	6c615dde-905d-44a1-8c1e-a786235bb2ea	\N	open	{workersComp,commercialAuto}	9cc7e6fd-3378-46e5-a98c-68425581ea80	21:32:03.280866+00	{"insurance": {"workersComp": {"mod": "92", "payroll": 400000, "governingClassCode": "2465", "additionalInformation": "no claims five years"}, "commercialAuto": {"heavyTrucks": 0, "lightTrucks": 3, "travelRadius": 10, "totalVehicles": 5, "privatePassenger": 2, "additionalInformation": "no claims 5 years"}}}
\.


--
-- TOC entry 4354 (class 0 OID 289348)
-- Dependencies: 204
-- Data for Name: profile; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.profile (profile_id, account_id, company_id, first_name, last_name, phone, email, create_date, title, specialty) FROM stdin;
c5b7a9f2-5540-4b05-a991-67320c0c62f5	170281f5-92d0-41ae-b408-8398429b3fbf	ee9c5b78-251f-4577-8601-8332fa0066e6	Brian	Yangas	14254570485	yangas@riskminute.com	2022-01-09 06:34:12.349126+00		\N
9cc7e6fd-3378-46e5-a98c-68425581ea80	152e756e-3cf0-42a3-ae70-56300544fbb9	61ef07b9-4e79-48df-9f2d-7f5773cee24c	Bryan	Colyer	2063906367	bryan@dohrins.com	2022-01-09 18:27:44.626972+00		\N
55d7f804-2d25-4ba9-9206-9c63e58f9bd8	\N	ee9c5b78-251f-4577-8601-8332fa0066e6	Bryan	Colyer	2063906367	bryan@riskminute.com	2022-01-11 21:36:45.705755+00	Underwriter	BOP
ce116476-a902-4f72-9db2-a83fe38e1798	c8c980a6-5523-4701-a26e-082080055c17	61ef07b9-4e79-48df-9f2d-7f5773cee24c	Marisol	Elizalde	2094781400	marisol@dohrins.com	2022-01-25 23:45:00.910707+00		\N
b7ec0868-17f7-4e77-8046-20c3a24536f3	5e49428f-ada2-4e22-9a59-d9c2e8dba35c	61ef07b9-4e79-48df-9f2d-7f5773cee24c	Robert	Richardson	209-712-7395	robert@dohrins.com	2022-01-27 01:03:24.816011+00		\N
\.


--
-- TOC entry 4358 (class 0 OID 289417)
-- Dependencies: 208
-- Data for Name: profile_contact; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.profile_contact (profile_contact_id, profile_id, company_id, invalid_date, created_by, created_date) FROM stdin;
ef717834-19c8-470d-82ce-280dc43e57e9	55d7f804-2d25-4ba9-9206-9c63e58f9bd8	61ef07b9-4e79-48df-9f2d-7f5773cee24c	\N	9cc7e6fd-3378-46e5-a98c-68425581ea80	2022-01-11 21:36:45.705755+00
\.


--
-- TOC entry 4355 (class 0 OID 289361)
-- Dependencies: 205
-- Data for Name: registration; Type: TABLE DATA; Schema: public; Owner: riskappadmin
--

COPY public.registration (registration_id, created_date, registration_code, expires_date, account_id, company_id, accept_date, email, roles) FROM stdin;
370523f5-d7a0-48a5-849f-27a8e7a41cc3	2022-01-25 01:36:14.895778+00	XBP60778	2022-01-26 17:36:15.132697+00	\N	ee9c5b78-251f-4577-8601-8332fa0066e6	\N	yangas@riskminute.com	{2b3d5d19-adb5-41d8-b0bf-e225e36d49af}
2f3eb567-da49-450b-bce6-247bf69bfcb2	2022-01-25 23:05:29.64711+00	UPM45511	2022-01-27 23:05:29.53894+00	\N	61ef07b9-4e79-48df-9f2d-7f5773cee24c	\N	bryan@dohrins.com	{1397b033-4553-40e7-b4a8-8f63a325f35a}
\.


--
-- TOC entry 4194 (class 2606 OID 289332)
-- Name: account account_pkey; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.account
    ADD CONSTRAINT account_pkey PRIMARY KEY (account_id);


--
-- TOC entry 4211 (class 2606 OID 289414)
-- Name: activity activity_pkey; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.activity
    ADD CONSTRAINT activity_pkey PRIMARY KEY (activity_id);


--
-- TOC entry 4196 (class 2606 OID 289338)
-- Name: application_role application_role_pkey; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.application_role
    ADD CONSTRAINT application_role_pkey PRIMARY KEY (application_role_id);


--
-- TOC entry 4207 (class 2606 OID 289382)
-- Name: account_role applicationroleaccount_id_ux; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.account_role
    ADD CONSTRAINT applicationroleaccount_id_ux UNIQUE (application_role_id, account_id);


--
-- TOC entry 4215 (class 2606 OID 289433)
-- Name: broker_account broker_account_pkey; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.broker_account
    ADD CONSTRAINT broker_account_pkey PRIMARY KEY (broker_account_id);


--
-- TOC entry 4198 (class 2606 OID 289347)
-- Name: company company_pkey; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.company
    ADD CONSTRAINT company_pkey PRIMARY KEY (company_id);


--
-- TOC entry 4200 (class 2606 OID 289360)
-- Name: profile email_ux; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.profile
    ADD CONSTRAINT email_ux UNIQUE (email);


--
-- TOC entry 4221 (class 2606 OID 289466)
-- Name: marketing_sheet_contact_note marketing_sheet_contact_note_pkey; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.marketing_sheet_contact_note
    ADD CONSTRAINT marketing_sheet_contact_note_pkey PRIMARY KEY (marketing_sheet_contact_note_id);


--
-- TOC entry 4219 (class 2606 OID 289456)
-- Name: marketing_sheet_contact marketing_sheet_contact_pkey; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.marketing_sheet_contact
    ADD CONSTRAINT marketing_sheet_contact_pkey PRIMARY KEY (marketing_sheet_contact_id);


--
-- TOC entry 4223 (class 2606 OID 289477)
-- Name: message message_pkey; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.message
    ADD CONSTRAINT message_pkey PRIMARY KEY (message_id);


--
-- TOC entry 4217 (class 2606 OID 289443)
-- Name: policy policy_pkey; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.policy
    ADD CONSTRAINT policy_pkey PRIMARY KEY (policy_id);


--
-- TOC entry 4213 (class 2606 OID 289423)
-- Name: profile_contact profile_contact_pkey; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.profile_contact
    ADD CONSTRAINT profile_contact_pkey PRIMARY KEY (profile_contact_id);


--
-- TOC entry 4203 (class 2606 OID 289358)
-- Name: profile profile_pkey; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.profile
    ADD CONSTRAINT profile_pkey PRIMARY KEY (profile_id);


--
-- TOC entry 4205 (class 2606 OID 289370)
-- Name: registration registration_id_pkey; Type: CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.registration
    ADD CONSTRAINT registration_id_pkey PRIMARY KEY (registration_id);


--
-- TOC entry 4208 (class 1259 OID 289416)
-- Name: activity_company_id_idx; Type: INDEX; Schema: public; Owner: riskappadmin
--

CREATE INDEX activity_company_id_idx ON public.activity USING btree (company_id);


--
-- TOC entry 4209 (class 1259 OID 289415)
-- Name: activity_entity_id_idx; Type: INDEX; Schema: public; Owner: riskappadmin
--

CREATE INDEX activity_entity_id_idx ON public.activity USING btree (entity_id);


--
-- TOC entry 4201 (class 1259 OID 289376)
-- Name: profile_email_idx; Type: INDEX; Schema: public; Owner: riskappadmin
--

CREATE UNIQUE INDEX profile_email_idx ON public.profile USING btree (email);


--
-- TOC entry 4227 (class 2606 OID 289444)
-- Name: policy FK_polic_broker_acount; Type: FK CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.policy
    ADD CONSTRAINT "FK_polic_broker_acount" FOREIGN KEY (broker_account_id) REFERENCES public.broker_account(broker_account_id);


--
-- TOC entry 4224 (class 2606 OID 289371)
-- Name: registration acountid_fk; Type: FK CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.registration
    ADD CONSTRAINT acountid_fk FOREIGN KEY (account_id) REFERENCES public.account(account_id);


--
-- TOC entry 4225 (class 2606 OID 289383)
-- Name: account_role ar_account_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.account_role
    ADD CONSTRAINT ar_account_id_fk FOREIGN KEY (account_id) REFERENCES public.account(account_id);


--
-- TOC entry 4226 (class 2606 OID 289388)
-- Name: account_role ar_aplication_role_fk; Type: FK CONSTRAINT; Schema: public; Owner: riskappadmin
--

ALTER TABLE ONLY public.account_role
    ADD CONSTRAINT ar_aplication_role_fk FOREIGN KEY (application_role_id) REFERENCES public.application_role(application_role_id);


--
-- TOC entry 4370 (class 0 OID 0)
-- Dependencies: 201
-- Name: TABLE account; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.account TO riskappweb;


--
-- TOC entry 4371 (class 0 OID 0)
-- Dependencies: 206
-- Name: TABLE account_role; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.account_role TO riskappweb;


--
-- TOC entry 4372 (class 0 OID 0)
-- Dependencies: 207
-- Name: TABLE activity; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.activity TO riskappweb;


--
-- TOC entry 4373 (class 0 OID 0)
-- Dependencies: 202
-- Name: TABLE application_role; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.application_role TO riskappweb;


--
-- TOC entry 4374 (class 0 OID 0)
-- Dependencies: 209
-- Name: TABLE broker_account; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.broker_account TO riskappweb;


--
-- TOC entry 4375 (class 0 OID 0)
-- Dependencies: 203
-- Name: TABLE company; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.company TO riskappweb;


--
-- TOC entry 4376 (class 0 OID 0)
-- Dependencies: 211
-- Name: TABLE marketing_sheet_contact; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.marketing_sheet_contact TO riskappweb;


--
-- TOC entry 4377 (class 0 OID 0)
-- Dependencies: 212
-- Name: TABLE marketing_sheet_contact_note; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.marketing_sheet_contact_note TO riskappweb;


--
-- TOC entry 4378 (class 0 OID 0)
-- Dependencies: 213
-- Name: TABLE message; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.message TO riskappweb;


--
-- TOC entry 4379 (class 0 OID 0)
-- Dependencies: 200
-- Name: TABLE pg_buffercache; Type: ACL; Schema: public; Owner: azure_superuser
--

GRANT ALL ON TABLE public.pg_buffercache TO riskappweb;


--
-- TOC entry 4380 (class 0 OID 0)
-- Dependencies: 199
-- Name: TABLE pg_stat_statements; Type: ACL; Schema: public; Owner: azure_superuser
--

GRANT ALL ON TABLE public.pg_stat_statements TO riskappweb;


--
-- TOC entry 4381 (class 0 OID 0)
-- Dependencies: 210
-- Name: TABLE policy; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.policy TO riskappweb;


--
-- TOC entry 4382 (class 0 OID 0)
-- Dependencies: 204
-- Name: TABLE profile; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.profile TO riskappweb;


--
-- TOC entry 4383 (class 0 OID 0)
-- Dependencies: 208
-- Name: TABLE profile_contact; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.profile_contact TO riskappweb;


--
-- TOC entry 4384 (class 0 OID 0)
-- Dependencies: 205
-- Name: TABLE registration; Type: ACL; Schema: public; Owner: riskappadmin
--

GRANT ALL ON TABLE public.registration TO riskappweb;


-- Completed on 2022-02-14 12:23:37

--
-- PostgreSQL database dump complete
--

