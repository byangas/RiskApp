
INSERT INTO application_role(application_role_id,name) 
                              values('dd797b4d-51fe-4161-ae9f-742ff32742b6', 'ROLE_SYSTEM_ADMIN')
							  ON CONFLICT DO NOTHING;
INSERT INTO application_role(application_role_id,name) 
                              values('f5a40d96-9418-4b38-99ef-ced5aa45b4af', 'ROLE_COMPANY_OWNER')
							  ON CONFLICT DO NOTHING;
						
INSERT INTO application_role(application_role_id,name) 
                              values('1397b033-4553-40e7-b4a8-8f63a325f35a', 'ROLE_BROKER')
							  ON CONFLICT DO NOTHING;
							  
INSERT INTO application_role(application_role_id,name) 
                              values('2b3d5d19-adb5-41d8-b0bf-e225e36d49af', 'ROLE_CARRIER_REP')
							  ON CONFLICT DO NOTHING;						  
								

							 
							 