INSERT INTO company (company_id,company_name,company_type,email_domain, company_website, company_logo)
VALUES ('61ef07b9-4e79-48df-9f2d-7f5773cee24c','Dohrmann Insurance','broker','dohrins.com', 'https://www.dorhins.com', 'dorhmann.png') 
ON CONFLICT DO NOTHING;

INSERT INTO public.company(
	 company_id, company_name, company_type, email_domain, company_website, company_logo)
	VALUES ('7df043d5-d1da-4593-8339-8ff049932ed4','CNA', 'carrier', 'cna.com', 'https://www.cna.com', 'cna.png')
	ON CONFLICT DO NOTHING; 
	
INSERT INTO public.company(
	 company_id, company_name, company_type, email_domain, company_website, company_logo)
	VALUES ('335305af-a5c7-40c2-b6cc-594165825a92','American Family Insurance', 'carrier', 'amfam.com', 'https://www.amfam.com', 'american_family.png')
	ON CONFLICT DO NOTHING;
	
INSERT INTO public.company(
   company_id, company_name, company_type, email_domain, company_website, company_logo)
VALUES ('882fac1b-768d-4d64-b744-3a6efca6edbc','Nationwide', 'carrier', 'nationwide.com', 'https://www.nationwide.com', 'nationwide.png')
ON CONFLICT DO NOTHING;

INSERT INTO public.company(
   company_id, company_name, company_type, email_domain, company_website, company_logo)
VALUES ('0d929dc0-50d4-499d-84ed-527791883daa','RiskMinute.com', 'carrier', 'riskminute.com', 'https://www.riskminute.com', 'riskminute.svg')
ON CONFLICT DO NOTHING;


INSERT INTO 
	profile ( profile_id,account_id,company_id,first_name,last_name,phone,email,create_date)
VALUES
('0d929dc0-50d4-499d-84ed-527791883daa','a7c5f204-1c6a-4bf4-8302-70b55620a75b',
 '61ef07b9-4e79-48df-9f2d-7f5773cee24c','Robby','Robinson','5555551212','rob@dohrins.com','2021-09-22 10:31:31.987974-07')
 ON CONFLICT DO NOTHING;
 
 /* password is brianbrian */
INSERT INTO account (account_id,password)
VALUES ('a7c5f204-1c6a-4bf4-8302-70b55620a75b','$2a$11$wWBpzXiufIhbAvUfWzEGzem8cOBrL.cT75xmVMOI.QmVyf8C.kps.') ON CONFLICT DO NOTHING;

INSERT INTO account_role (account_id,application_role_id)
VALUES ('a7c5f204-1c6a-4bf4-8302-70b55620a75b','1397b033-4553-40e7-b4a8-8f63a325f35a')
ON CONFLICT DO NOTHING;
