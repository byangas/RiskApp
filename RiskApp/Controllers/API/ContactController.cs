using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiskApp.Models.User;
using RiskApp.Services;
using RiskApp.Extensions;
using RiskApp.Models;

namespace RiskApp.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ContactService contactService;
        private readonly ProfileService profileService;

        public ContactController(ContactService contactService, ProfileService profileService)
        {
            this.contactService = contactService;
            this.profileService = profileService;
        }

        [HttpGet("")]
        public ActionResult<Contact> GetContact([FromQuery] string email, [FromQuery] Guid? contactId)
        {

            var contact = contactService.GetContactByEmailOrId(User.CurrentUserCompanyId(), email, contactId);
            if (contact != null)
            {
                return Ok(contact);
            }
            else
            {
                return NoContent();
            }
        }


        /// <summary>
        /// returns list of carrier reps and underwriter. Can filter by particular carrier.
        /// </summary>
        /// <param name="companyId">The id of the company to filter by. If the user wants to only see the contacts from a particular carrier</param>
        /// <param name="showMyCompanyOnly">If the user wants to see only the 'company contacts' or not.</param>
        /// <returns>always return an array, if no resutls, the array will be empty.</returns>
        [HttpGet("carrier")]
        public ActionResult<IEnumerable<Contact>> GetAllCarrierContacts(
            [FromQuery] Guid? companyId = null, 
            [FromQuery] string search = null,  
            [FromQuery] bool showMyCompanyOnly = false, 
            [FromQuery] string sortBy= "company_asc")
        {

             Guid currentCompany = User.CurrentUserCompanyId();

            IEnumerable<Contact> contacts = contactService.GetBrokersCarrierContacts(currentCompany, showMyCompanyOnly, search, sortBy, companyId);
            return Ok(contacts);
        }


        [HttpPost()]
        public ActionResult<Profile> Create([FromBody] ProfileCreate profile)
        {
            // grab the current company id
            Guid currentCompanyId = User.CurrentUserCompanyId();
            Guid currentUserProfileId = User.CurrentUserId();
            Profile newProfile = contactService.CreateContact(profile, currentCompanyId, currentUserProfileId);

            return Created("some uri", newProfile);
        }



        /// <summary>
        /// Does not really delete the contact, but just clears out the company/contact relationship. A user cannot delete a profile.
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void DeleteCompanyContact([FromRoute] Guid id)
        {
            // grab the current company id
            Guid currentCompanyId = User.CurrentUserCompanyId();
            contactService.RemoveCompanyContact(currentCompanyId, id);
        }

        [HttpPut("{id}")]
        public void UpdateContact([FromRoute] Guid id, [FromBody] Profile profile)
        {
            //just to make sure it's the one from the url
            profile.Id = id;
            profileService.UpdateProfile(profile);
        }

        [HttpPost("{id}")]
        public void AddCompanyContact([FromRoute] Guid id)
        {
            Guid currentCompanyId = User.CurrentUserCompanyId();
            Guid currentUserProfileId = User.CurrentUserId();
            contactService.AddCompanyContact(currentCompanyId, currentUserProfileId, id);
        }

    }
}
