DO $$
BEGIN

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS account
(
    account_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    password character varying(256) NOT NULL,
    create_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT account_pkey PRIMARY KEY (account_id)
);

CREATE TABLE IF NOT EXISTS application_role
(
    application_role_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    name character varying(256) NOT NULL,
    CONSTRAINT application_role_pkey PRIMARY KEY (application_role_id)
);

CREATE TABLE IF NOT EXISTS company
(
    company_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    company_name character varying(1024) NOT NULL,
	company_type character varying(1024) NOT NULL,
    email_domain character varying(2048) NOT NULL,
	company_website character varying(1024) NOT NULL,
	company_logo character varying(1024) NOT NULL,
    CONSTRAINT company_pkey PRIMARY KEY (company_id)
);


CREATE TABLE IF NOT EXISTS profile
(
    profile_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    account_id uuid,
    company_id uuid,
    first_name character varying(255),
    last_name character varying(255),
    phone character varying,
    email character varying(255) NOT NULL,
    create_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    title character varying(255)   NOT NULL DEFAULT ''::character varying,
    specialty character varying(255),
    CONSTRAINT profile_pkey PRIMARY KEY (profile_id),
    CONSTRAINT email_ux UNIQUE (email)    
);

CREATE TABLE IF NOT EXISTS registration
(
    registration_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    created_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    registration_code character varying(255)   NOT NULL,
    expires_date timestamp with time zone NOT NULL,
    account_id uuid,
    company_id uuid,
    accept_date timestamp with time zone,
    email character varying(255)   NOT NULL,
	roles uuid[] NOT NULL,
    CONSTRAINT registration_id_pkey PRIMARY KEY (registration_id),
    CONSTRAINT acountid_fk FOREIGN KEY (account_id)
        REFERENCES account (account_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE UNIQUE INDEX IF NOT EXISTS profile_email_idx
    ON profile USING btree
    (email   ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS account_role
(
    account_id uuid NOT NULL,
	application_role_id uuid NOT NULL,
    created_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT applicationroleaccount_id_ux UNIQUE (application_role_id, account_id),
    CONSTRAINT ar_account_id_fk FOREIGN KEY (account_id)
        REFERENCES account (Account_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    CONSTRAINT ar_aplication_role_fk FOREIGN KEY (application_role_id)
        REFERENCES application_role (application_role_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
);



IF NOT EXISTS (SELECT * from pg_type where typname = 'action_type') THEN
	CREATE TYPE action_type AS ENUM ('create', 'update', 'delete', 'message', 'related'); 
END IF;
	

	
CREATE TABLE IF NOT EXISTS activity
(
    activity_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    entity_id uuid NOT NULL,
    activity_type action_type NOT NULL,
    created_by uuid NOT NULL,
    created_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    description character varying(2056)   NOT NULL,
    company_id uuid,
    CONSTRAINT activity_pkey PRIMARY KEY (activity_id)
);

CREATE INDEX IF NOT EXISTS activity_entity_id_idx ON activity (entity_id ASC NULLS LAST);
CREATE INDEX IF NOT EXISTS activity_company_id_idx ON activity (company_id ASC NULLS LAST);

CREATE TABLE IF NOT EXISTS profile_contact
(
    profile_contact_id uuid NOT NULL DEFAULT uuid_generate_v4(),
	profile_id uuid NOT NULL,
    company_id uuid NOT NULL,
    invalid_date timestamp with time zone,
    created_by uuid NOT NULL,
    created_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT profile_contact_pkey PRIMARY KEY (profile_contact_id)
);

/* 
this is the table for storing the "Accounts" for brokers. Not 
the same as the 'account' table.
**/
CREATE TABLE IF NOT EXISTS broker_account
(
    broker_account_id uuid NOT NULL DEFAULT uuid_generate_v4(),
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
    created_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    information text NULL,
    PRIMARY KEY ( broker_account_id )
);

CREATE TABLE IF NOT EXISTS policy
(
    policy_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    broker_account_id uuid NOT NULL,
    renewal_date time with time zone,
    status character varying(50)  NOT NULL,
    insurance_types character varying(50)[]  NOT NULL,
    created_by_profile_id uuid NOT NULL,
    created_date time with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    detail jsonb NOT NULL,
    CONSTRAINT policy_pkey PRIMARY KEY (policy_id),
    CONSTRAINT "FK_polic_broker_acount" FOREIGN KEY (broker_account_id)
        REFERENCES public.broker_account (broker_account_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
);

CREATE TABLE IF NOT EXISTS marketing_sheet_contact
(
    marketing_sheet_contact_id uuid NOT NULL DEFAULT uuid_generate_v4(),
	policy_id uuid NOT NULL,
	profile_id uuid NOT NULL,
    status character varying(50) NOT NULL DEFAULT 'new',
    created_by_profile_id uuid NOT NULL,
	premium int NULL,
	commission int NULL, 
    created_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (marketing_sheet_contact_id)
);

CREATE TABLE IF NOT EXISTS marketing_sheet_contact_note
(
    marketing_sheet_contact_note_id uuid NOT NULL DEFAULT uuid_generate_v4(),
	marketing_sheet_contact_id uuid NOT NULL,
	note text NOT NULL,
    created_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (marketing_sheet_contact_note_id)
);


CREATE TABLE IF NOT EXISTS message
(
    message_id uuid NOT NULL DEFAULT uuid_generate_v4(),
	sender_profile_id uuid NOT NULL,
	send_to_profile_id uuid NOT NULL,
	subject_id uuid NOT NULL,
	message_type character varying(25) NOT NULL,
	message text NOT NULL,
    created_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
	status character varying(25) NOT NULL DEFAULT 'new',
	read_date timestamp with time zone,
    PRIMARY KEY (message_id)
);

END; $$





	



