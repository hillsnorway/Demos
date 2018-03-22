using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using DVDLibWebAPI.Data;
using DVDLibWebAPI.Data.Models;
using DVDLibWebAPI.Models;

namespace DVDLibWebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DvdController : ApiController
    {
        [Route("dvds")]
        [AcceptVerbs("GET")]
        public IHttpActionResult All()
        {
            return Ok(DvdRepositoryFactory.GetRepository().GetAll().Select(kvp => kvp.Value).ToList());
        }

        [Route("dvds/title/{title}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult AllByTitle(string title)
        {
            return Ok(DvdRepositoryFactory.GetRepository().GetAll().Where(kvp => kvp.Value.title.ToUpper().Contains(title.ToUpper())).Select(kvp => kvp.Value).ToList());
        }

        [Route("dvds/year/{realeaseYear}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult AllByYear(string realeaseYear)
        {
            return Ok(DvdRepositoryFactory.GetRepository().GetAll().Where(kvp => kvp.Value.realeaseYear.ToUpper().Contains(realeaseYear.ToUpper())).Select(kvp => kvp.Value).ToList());
        }

        [Route("dvds/director/{director}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult AllByDirector(string director)
        {
            return Ok(DvdRepositoryFactory.GetRepository().GetAll().Where(kvp => kvp.Value.director.ToUpper().Contains(director.ToUpper())).Select(kvp => kvp.Value).ToList());
        }

        [Route("dvds/rating/{rating}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult AllByRating(string rating)
        {
            return Ok(DvdRepositoryFactory.GetRepository().GetAll().Where(kvp => kvp.Value.rating.ToUpper().Contains(rating.ToUpper())).Select(kvp => kvp.Value).ToList());
        }

        [Route("dvd/{dvdID}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult Get(int dvdID)
        {
            DVDdb dvd = DvdRepositoryFactory.GetRepository().Get(dvdID);

            if (dvd == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(dvd);
            }
        }

        [Route("dvd")]
        [AcceptVerbs("POST")]
        public IHttpActionResult Add(AddDvdRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DVDdb dvd = new DVDdb()
            {
                title = request.title,
                realeaseYear = request.realeaseYear,
                director = request.director,
                rating = request.rating,
                notes = request.notes //Not Required
            };

            DvdRepositoryFactory.GetRepository().Add(dvd);
            return Created($"dvds/get/{dvd.dvdId}", dvd);
        }

        [Route("dvd/{dvdID}")]
        [AcceptVerbs("PUT")]
        public IHttpActionResult Update(UpdateDvdRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DVDdb dvd = DvdRepositoryFactory.GetRepository().Get(request.dvdID);

            if(dvd == null)
            {
                return NotFound();
            }

            dvd.title = request.title;
            dvd.realeaseYear = request.realeaseYear;
            dvd.director = request.director;
            dvd.rating = request.rating;
            dvd.notes = request.notes; //Not Required

            DvdRepositoryFactory.GetRepository().Edit(dvd, dvd.dvdId);
            return Ok(dvd);
        }

        [Route("dvd/{dvdID}")]
        [AcceptVerbs("DELETE")]
        public IHttpActionResult Delete(int dvdID)
        {
            DVDdb dvd = DvdRepositoryFactory.GetRepository().Get(dvdID);

            if (dvd == null)
            {
                return NotFound();
            }

            DvdRepositoryFactory.GetRepository().Delete(dvdID);
            return Ok();
        }
    }
}
