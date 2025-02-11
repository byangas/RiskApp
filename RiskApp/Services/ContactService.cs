using RiskApp.Data;
using RiskApp.Models;
using RiskApp.Models.User;
using RiskApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Services
{
    public class ContactService
    {
        private readonly ContactRepository contactRepository;
        private readonly ConnectionManager connectionManager;
        private readonly ProfileContactRepository profileContactRepository;
        private readonly ProfileRepository profileRepository;
        private readonly CarrierAppointmentRepository carrierAppointmentRepository;

        public ContactService(ContactRepository contactRepository,
            ConnectionManager connectionManager,
            ProfileContactRepository profileContactRepository,
            ProfileRepository profileRepository,
            CarrierAppointmentRepository carrierAppointmentRepository)
        {
            this.contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            this.connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
            this.profileContactRepository = profileContactRepository ?? throw new ArgumentNullException(nameof(profileContactRepository));
            this.profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
            this.carrierAppointmentRepository = carrierAppointmentRepository ?? throw new ArgumentNullException(nameof(carrierAppointmentRepository));
        }


        public Contact GetContactByEmailOrId(Guid companyId, string email, Guid? profileId )
        {
            Contact contact = contactRepository.GetContact(companyId, email, profileId);
            return contact;
        }

        public void AddCompanyContact(Guid currentCompanyId, Guid currentUserProfileId, Guid profileId)
        {
            ProfileContact profileContact = profileContactRepository.GetByProfileAndCompany(currentCompanyId, profileId);
            //if the record exists, it must be that the expired date was set, so clear it out.
            if(profileContact != null)
            {
                profileContactRepository.ClearExpiration(profileContact.Id);
            }
            else
            {
                // create a new entry
                profileContactRepository.CreateCompanyContact(profileId, currentCompanyId, currentUserProfileId);
                carrierAppointmentRepository.AddAppointmentForBroker(profileContact.CompanyId, currentCompanyId, currentUserProfileId);
            }
        }

        public Profile CreateContact(ProfileCreate createProfile, Guid currentCompanyId, Guid currentUserProfileId)
        {

            connectionManager.BeginTransaction();
            try
            {
                profileRepository.CreateProfile(createProfile);

                // get the newly created profile so we can create a "contact" relationship
                Contact newProfile = contactRepository.GetContact(currentCompanyId, createProfile.Email);
                profileContactRepository.CreateCompanyContact(newProfile.Id, currentCompanyId, currentUserProfileId);
                carrierAppointmentRepository.AddAppointmentForBroker(newProfile.CompanyId, currentCompanyId, currentUserProfileId);

                connectionManager.CommitTransaction();

                return newProfile;
            }
            catch (Exception)
            {
                connectionManager.Rollback();
                throw;
            }
        }

        public IEnumerable<Contact> GetBrokersCarrierContacts(Guid currentUsersCompany, bool showOnlyCurrentUsersCompanyContacts, string search, string sortBy, Guid? companyId)
        {
            return contactRepository.BrokerCompanyContacts(currentUsersCompany, showOnlyCurrentUsersCompanyContacts, search, sortBy, companyId);
        }

        public void RemoveCompanyContact(Guid currentCompanyId, Guid profileId)
        {
            ProfileContact profileContact = profileContactRepository.GetByProfileAndCompany(currentCompanyId, profileId);
            if(profileContact != null )
            {
                //really just sets the expired date
                profileContactRepository.RemoveCompanyContact(profileContact.Id);
            }
            else
            {
                throw new InvalidOperationException("Contact not found");
            }
        }
    }
}
