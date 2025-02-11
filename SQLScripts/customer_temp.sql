-- Table: public.customer_temp

-- DROP TABLE public.customer_temp;

CREATE TABLE public.customer_temp
(
	customer_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    customer_firm_name character varying(1000),
	customer_name	character varying(1000),
	customer_number	character varying(120),
	city	character varying(120),
	state	character varying(120),
	customer_contact character varying(120),	
	customer_phone_residence character varying(120),
	customer_phone_business	character varying(120),
	customer_email	character varying(120),
	customer_phone_mobile	character varying(120)
	
)


 -- DROP table public.policy_temp
CREATE TABLE  public.policy_temp
(
	policy_id uuid NOT NULL DEFAULT uuid_generate_v4(),
	customer_number	character varying(120),
    policy_expiration_date	 timestamp with time zone,
	policy_number	character varying(120),
	policy_effective_date	 timestamp with time zone,
	policy_carrier	character varying(200),
	customer_number	character varying(120),
	policy_type	character varying(120),
	policy_description	character varying(120),
	policy_total_cost character varying(120),	
	policy_representative	character varying(120),
	policy_lines_of_business character varying(120),
	ams_360_id uuid

)

