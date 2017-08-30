using System.Data;
using Relisten.Api.Models;
using Dapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Relisten.Data
{
    public class SourceTrackService : RelistenDataServiceBase
    {
        public SourceTrackService(DbService db) : base(db) { }

		public async Task<SourceTrack> ForId(int id)
		{
			return (await ForIds(new[] { id })).FirstOrDefault();
		}

		public Task<IEnumerable<SourceTrack>> ForIds(IList<int> ids)
		{
			return db.WithConnection(con => con.QueryAsync<SourceTrack>(@"
                SELECT
                    t.*
                FROM
                    source_tracks t
                WHERE
                    t.id = ANY(@trackIds)
            ", new { trackIds = ids }));
		}

		public async Task<IEnumerable<SourceTrack>> InsertAll(IEnumerable<SourceTrack> songs)
        {
            return await db.WithConnection(async con =>
            {
                var inserted = new List<SourceTrack>();

                foreach (var song in songs)
                {
                    inserted.Add(await con.QuerySingleAsync<SourceTrack>(@"
                        INSERT INTO
                            source_tracks

                            (
                                source_id,
                                source_set_id,
                                track_position,
                                duration,
                                title,
                                slug,
                                mp3_url,
								mp3_md5,
                                flac_url,
								flac_md5,
                                updated_at,
                                artist_id
                            )
                        VALUES
                            (
                                @source_id,
                                @source_set_id,
                                @track_position,
                                @duration,
                                @title,
                                @slug,
                                @mp3_url,
                                @mp3_md5,
                                @flac_url,
                                @flac_md5,
                                @updated_at,
                                @artist_id
                            )
                        RETURNING *
                    ", song));
                }

                return inserted;
            });
        }
    }
}