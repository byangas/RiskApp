

ALTER TABLE public.company
    ALTER COLUMN company_website DROP NOT NULL;

ALTER TABLE public.company
    ALTER COLUMN company_logo DROP NOT NULL;
	

CREATE TABLE IF NOT EXISTS carrier_appointment
(    
	carrier_appointment_id uuid NOT NULL DEFAULT uuid_generate_v4(),
	brokerage_company_id uuid NOT NULL,
    carrier_company_id uuid NOT NULL,
    created_by uuid NOT NULL,
	brokerage_notes character varying(1024),
    created_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT carrier_appointment_pkey PRIMARY KEY (carrier_appointment_id),
	CONSTRAINT broker_carrier_unique UNIQUE (brokerage_company_id, carrier_company_id)
);
	
ALTER TABLE IF EXISTS public.policy DROP COLUMN IF EXISTS broker_account_id;
ALTER TABLE IF EXISTS public.policy DROP CONSTRAINT IF EXISTS "FK_polic_broker_acount";

CREATE TABLE IF NOT EXISTS customer
(
    customer_id uuid NOT NULL DEFAULT uuid_generate_v4(),
	company_id uuid NOT NULL,
	customer_name character varying(200),
	firm_name character varying(200),
	primary_contact_name character varying(200),
	industry character varying(200) NOT NULL,
	industry_details character varying(1000) NOT NULL,
	address character varying(200) NOT NULL,
	city character varying(200) NOT NULL,
    state character varying(50) NOT NULL,
	zip character varying(25) NOT NULL,
	email character varying(200),
	business_phone character varying(200),
	residence_phone character varying(200),
	mobile_phone character varying(200),
    created_by_profile_id uuid NOT NULL,
    created_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    information text NULL,
    PRIMARY KEY ( customer_id )
);
DELETE from policy;

ALTER TABLE public.policy
    ADD COLUMN customer_id uuid NOT NULL;
	
ALTER TABLE public.policy
    ADD COLUMN description character varying(1000) NOT NULL;
	
ALTER TABLE public.policy
    ALTER COLUMN detail DROP NOT NULL;
	
ALTER TABLE IF EXISTS public.policy DROP COLUMN IF EXISTS renewal_date;

ALTER TABLE IF EXISTS public.policy DROP COLUMN IF EXISTS created_date;
	
ALTER TABLE IF EXISTS public.policy
    ADD COLUMN renewal_date timestamp with time zone;

ALTER TABLE IF EXISTS public.policy
    ADD COLUMN created_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP;
	
	
ALTER TABLE IF EXISTS public.marketing_sheet_contact
    ADD CONSTRAINT marketing_sheet_contact_policy_contact_unique UNIQUE (policy_id, profile_id);
	
ALTER TABLE IF EXISTS public.profile
    ADD COLUMN mobile_phone character varying(50);
	
/*****
April 26 2022
******/

CREATE TABLE public.policy_note
(
    policy_note_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    policy_id uuid NOT NULL,
    created_by uuid NOT NULL,
    created_date timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    note text NOT NULL,
    PRIMARY KEY (policy_note_id)
);

GRANT ALL ON TABLE public.policy_note TO riskappweb;

ALTER TABLE public.policy
    ALTER COLUMN renewal_date TYPE date;

